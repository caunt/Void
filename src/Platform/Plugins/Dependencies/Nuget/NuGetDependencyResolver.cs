using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Nito.Disposables.Internals;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Void.Proxy.Api.Plugins.Dependencies;
using ZLinq;

namespace Void.Proxy.Plugins.Dependencies.Nuget;

public partial class NuGetDependencyResolver(ILogger<NuGetDependencyResolver> logger, InvocationContext context) : INuGetDependencyResolver
{
    private static readonly Option<string[]> _repositoriesOption = new(["--repository", "-r"], "Provides a URI to NuGet repository [--repository https://nuget.example.com/v3/index.json or --repository https://username:password@nuget.example.com/v3/index.json].");
    private static readonly string FrameworkName = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
        ?? throw new InvalidOperationException("Cannot determine the target framework.");

    private static readonly SourceRepository DefaultRepository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
    private static readonly SourceCacheContext Cache = new();
    private static readonly string PackagesPath = Path.Combine(Directory.GetCurrentDirectory(), SettingsUtility.DefaultGlobalPackagesFolderPath);

    private readonly NuGet.Common.ILogger _nugetLogger = new NuGetLogger(logger);
    private readonly HashSet<string> _repositories = [];

    public static void RegisterOptions(Command command)
    {
        command.AddOption(_repositoriesOption);
    }

    public void AddRepository(string uri)
    {
        _repositories.Add(uri);
    }

    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        logger.LogTrace("Resolving {AssemblyName} dependency", assemblyName.Name);

        if (assemblyName.FullName.StartsWith(nameof(Void) + '.'))
        {
            logger.LogCritical("Void package {AssemblyName} shouldn't be searched in NuGet", assemblyName.Name);
            return null;
        }

        if (ResolveAssemblyFromOfflineNuGetAsync(assemblyName).GetAwaiter().GetResult() is { } offlineAssemblyPath)
        {
            var assembly = context.LoadFromAssemblyPath(offlineAssemblyPath);

            if (AreEqual(assembly.GetName(), assemblyName))
                return assembly;
        }

        if (ResolveAssemblyFromOnlineNuGetAsync(assemblyName).GetAwaiter().GetResult() is { } onlineAssemblyPath)
        {
            var assembly = context.LoadFromAssemblyPath(onlineAssemblyPath);

            if (AreEqual(assembly.GetName(), assemblyName))
                return assembly;
        }

        return null;

        static bool AreEqual(AssemblyName assemblyName1, AssemblyName assemblyName2) => assemblyName1.Name?.Equals(assemblyName2.Name) ?? false;
    }

    private async Task<string?> ResolveAssemblyFromOfflineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assemblyName.Name))
                return null;

            // TODO: directory name does not guarantee NuGet package name 
            var localPackagePath = Path.Combine(PackagesPath, assemblyName.Name.ToLower());

            if (!Directory.Exists(localPackagePath))
            {
                logger.LogTrace("Dependency {DependencyName} not found in the offline NuGet cache", assemblyName.Name);

                // some packages easily resolved with just removed suffixes
                if (!assemblyName.Name.Contains('.'))
                    return null;

                var prefix = assemblyName.Name[..assemblyName.Name.LastIndexOf('.')];
                var variant = (AssemblyName)assemblyName.Clone();
                variant.Name = prefix;

                return await ResolveAssemblyFromOfflineNuGetAsync(variant, cancellationToken);
            }

            var availableVersions = Directory.GetDirectories(localPackagePath).Select(Path.GetFileName).WhereNotNull().ToArray();

            if (availableVersions.Length == 0)
            {
                logger.LogWarning("No versions found for {DependencyName} in the offline NuGet cache", assemblyName.Name);
                return null;
            }

            var identities = availableVersions.AsValueEnumerable().Select(version => new PackageIdentity(assemblyName.Name, NuGetVersion.Parse(version))).ToArray();
            var identity = SelectBestNuGetPackageVersion(identities, assemblyName.Version);

            if (identity == null)
            {
                logger.LogError("No matching version found for {DependencyName} in the offline NuGet cache", assemblyName.Name);
                return null;
            }

            var packagePath = Path.Combine(PackagesPath, identity.Id.ToLower(), identity.Version.ToString());
            var packageReader = new PackageFolderReader(packagePath);

            var frameworks = await packageReader.GetLibItemsAsync(cancellationToken);
            var targetFramework = NuGetFramework.ParseFrameworkName(FrameworkName, new DefaultFrameworkNameProvider());

            foreach (var framework in frameworks)
            {
                if (!DefaultCompatibilityProvider.Instance.IsCompatible(targetFramework, framework.TargetFramework))
                    continue;

                var assembly = framework.Items.FirstOrDefault(fileName => Path.GetFileName(fileName).Equals(assemblyName.Name + ".dll", StringComparison.InvariantCultureIgnoreCase)) ?? framework.Items.FirstOrDefault();

                return assembly is null
                    ? throw new FileNotFoundException($"Dependency {assemblyName.Name} was found in the offline NuGet cache but the file cannot be located")
                    : Path.Combine(packagePath, assembly);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to resolve {DependencyName} from the offline NuGet cache", assemblyName.Name);
        }

        return null;
    }

    private async Task<string?> ResolveAssemblyFromOnlineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken = default)
    {
        try
        {
            var environmentVariableRepositories = UnescapedSemicolonRegex().Split(Environment.GetEnvironmentVariable("VOID_NUGET_REPOSITORIES") ?? "")
                .AsValueEnumerable()
                .Select(repo => repo.Replace(@"\;", ";"))
                .ToArray();
            var repositories = environmentVariableRepositories.Concat(_repositories.Concat(context.ParseResult.GetValueForOption(_repositoriesOption) ?? []))
                .Select(source =>
                {
                    if (!Uri.TryCreate(source, UriKind.Absolute, out var uri))
                        return null;

                    var url = new UriBuilder(uri)
                    {
                        UserName = "",
                        Password = ""
                    }.Uri.ToString();

                    var repository = Repository.Factory.GetCoreV3(url);

                    if (string.IsNullOrWhiteSpace(uri.UserInfo))
                        return repository;

                    var parts = Uri.UnescapeDataString(uri.UserInfo).Split(':');

                    if (parts.Length is not 2)
                        return repository;

                    repository.PackageSource.Credentials = PackageSourceCredential.FromUserInput(url, parts[0], parts[1], true, null);
                    return repository;
                })
                .Append(DefaultRepository)
                .WhereNotNull();

            foreach (var repository in repositories)
            {
                var identity = await TryResolveNuGetIdentityAsync(repository, assemblyName, cancellationToken);

                if (identity is null)
                {
                    logger.LogTrace("Dependency {DependencyName} not found in {Repository}", assemblyName.Name, repository.PackageSource.Name);
                    return null;
                }

                var packagePath = Path.Combine(PackagesPath, identity.Id.ToLower(), identity.Version.ToString());

                if (Directory.Exists(packagePath))
                    break;

                await TryDownloadNuGetPackageAsync(repository, identity, cancellationToken);

                if (!Directory.Exists(packagePath))
                {
                    logger.LogWarning("Dependency {DependencyName} cannot be downloaded from {Repository}", identity.Id, repository.PackageSource.Name);
                    return null;
                }

                var packageReader = new PackageFolderReader(packagePath);
                var frameworks = await packageReader.GetLibItemsAsync(cancellationToken);
                var targetFramework = NuGetFramework.ParseFrameworkName(FrameworkName, new DefaultFrameworkNameProvider());

                foreach (var framework in frameworks)
                {
                    if (!DefaultCompatibilityProvider.Instance.IsCompatible(targetFramework, framework.TargetFramework))
                        continue;

                    var assembly = framework.Items.FirstOrDefault(fileName => Path.GetFileName(fileName).Equals(assemblyName.Name + ".dll", StringComparison.InvariantCultureIgnoreCase)) ?? framework.Items.FirstOrDefault();

                    return assembly is null
                        ? throw new FileNotFoundException($"Dependency {identity.Id} was downloaded from NuGet but file cannot be located")
                        : Path.Combine(packagePath, assembly);
                }
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Dependency {PackageId} cannot be resolved from NuGet", assemblyName.Name);
        }

        return null;
    }

    private async Task TryDownloadNuGetPackageAsync(SourceRepository repository, PackageIdentity identity, CancellationToken cancellationToken)
    {
        try
        {
            using var result = await PackageDownloader.GetDownloadResourceResultAsync(repository, identity, new PackageDownloadContext(Cache), PackagesPath, _nugetLogger, cancellationToken);
            logger.LogTrace("Downloaded {PackageId} version {PackageVersion} dependency from {Repository}", identity.Id, identity.Version, repository.PackageSource.Name);
        }
        catch (FatalProtocolException exception)
        {
            logger.LogError("Dependency {PackageId} cannot be downloaded from {Repository}: {Reason}", identity.Id, repository.PackageSource.Name, exception.Message);
        }
        catch (RetriableProtocolException exception)
        {
            logger.LogWarning("Dependency {PackageId} loading from {Repository} was cancelled: {Message}", identity.Id, repository.PackageSource.Name, exception.Message);
        }
    }

    private async Task<PackageIdentity?> TryResolveNuGetIdentityAsync(SourceRepository repository, AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        logger.LogTrace("Looking for dependency {DependencyName} as Identity in NuGet", assemblyName.Name);
        var identity = await TryResolveNuGetPackageIdAsync(repository, assemblyName, cancellationToken);

        if (identity is not null)
            return identity;

        logger.LogTrace("Looking for dependency {DependencyName} with TryGet in NuGet", assemblyName.Name);
        identity = await TryResolveNuGetPackageSearchAsync(repository, assemblyName, cancellationToken);

        if (identity is not null)
            return identity;

        logger.LogTrace("Dependency {DependencyName} not found in NuGet", assemblyName.Name);
        return null;
    }

    private async Task<PackageIdentity?> TryResolveNuGetPackageIdAsync(SourceRepository repository, AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(assemblyName.Name))
            return null;

        var packages = await GetNuGetPackageVersionAsync(repository, assemblyName.Name, cancellationToken);
        var best = SelectBestNuGetPackageVersion(packages.AsValueEnumerable().Select(package => package.Identity).ToArray(), assemblyName.Version);

        return best;
    }

    private async Task<PackageIdentity?> TryResolveNuGetPackageSearchAsync(SourceRepository repository, AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        var packageSearchResource = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
        var packageSearchResults = await packageSearchResource.SearchAsync(assemblyName.Name, new SearchFilter(true), 0, 1, _nugetLogger, cancellationToken);

        // actually always 1
        foreach (var packageSearchResult in packageSearchResults)
        {
            var packages = await GetNuGetPackageVersionAsync(repository, packageSearchResult.Identity.Id, cancellationToken);
            var best = SelectBestNuGetPackageVersion(packages.AsValueEnumerable().Select(package => package.Identity).ToArray(), assemblyName.Version);

            return best;
        }

        return null;
    }

    private PackageIdentity? SelectBestNuGetPackageVersion(IEnumerable<PackageIdentity> identities, Version? assemblyVersion)
    {
        PackageIdentity? result = null;

        foreach (var identity in identities)
        {
            if (result is null)
            {
                result = identity;
                continue;
            }

            if (!identity.HasVersion)
                continue;

            if (identity.Version.CompareTo(result.Version) <= 0)
                continue;

            if (assemblyVersion is null)
                result = identity;
            else if (assemblyVersion.Major == identity.Version.Major)
                result = identity;
        }

        if (result is null)
            return null;

        logger.LogTrace("Dependency {DependencyName} selected best version {DependencyVersion}", result.Id, result.Version);
        return result;
    }

    private async Task<IEnumerable<IPackageSearchMetadata>> GetNuGetPackageVersionAsync(SourceRepository repository, string packageId, CancellationToken cancellationToken)
    {
        var packageMetadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await packageMetadataResource.GetMetadataAsync(packageId, true, false, Cache, _nugetLogger, cancellationToken);
    }

    [GeneratedRegex(@"(?<!\\);")]
    private static partial Regex UnescapedSemicolonRegex();
}
