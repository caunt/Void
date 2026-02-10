using System.CommandLine;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Nito.Disposables.Internals;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Void.Proxy.Api;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Dependencies.Nuget;

public partial class NuGetDependencyResolver(ILogger<NuGetDependencyResolver> logger, IRunOptions runOptions, IConsoleService console, HttpClient httpClient) : INuGetDependencyResolver, IEventListener
{
    private static readonly Option<string[]> RepositoryOption = new("--repository", "-r")
    {
        Description = "Provides a URI to NuGet repository.\nExamples:\n--repository https://nuget.example.com/v3/index.json\n--repository https://username:password@nuget.example.com/v3/index.json"
    };
    private static readonly Option<bool> EnableNugetLoggingOption = new("--enable-nuget-logging")
    {
        Description = "Enables detailed logging for NuGet operations.",
        DefaultValueFactory = (argumentResult) => false
    };

    private static readonly string FrameworkName = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? throw new InvalidOperationException("Cannot determine the target framework.");

    private static readonly SourceRepository DefaultRepository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
    private static readonly SourceCacheContext Cache = new();

    private readonly string _packagesPath = Path.Combine(runOptions.WorkingDirectory, SettingsUtility.DefaultGlobalPackagesFolderPath);
    private readonly NuGet.Common.ILogger _nugetLogger = console.GetOptionValue(EnableNugetLoggingOption) ? new NuGetLogger(logger) : NullLogger.Instance;
    private readonly HashSet<string> _repositories = [];

    private IEnumerable<string> UriRepositories => UnescapedSemicolonRegex().Split(Environment.GetEnvironmentVariable("VOID_NUGET_REPOSITORIES") ?? "").Select(repo => repo.Replace(@"\;", ";")).Concat(_repositories.Concat(console.GetOptionValue(RepositoryOption) ?? [])).Where(uri => !string.IsNullOrWhiteSpace(uri));
    private IEnumerable<SourceRepository> Repositories
    {
        get
        {
            return UriRepositories.Select(source =>
            {
                if (!Uri.TryCreate(source, UriKind.Absolute, out var uri))
                    return null;

                var url = new UriBuilder(uri) { UserName = "", Password = "" }.Uri.ToString();
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
        }
    }

    [Subscribe]
    public async ValueTask OnProxyStarting(ProxyStartingEvent @event, CancellationToken cancellationToken)
    {
        console.EnsureOptionDiscovered(RepositoryOption);
        console.EnsureOptionDiscovered(EnableNugetLoggingOption);

        try
        {
            await ProbeRepositoriesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Failed to probe NuGet repositories");
        }
    }

    public void AddRepository(string uri)
    {
        _repositories.Add(uri);
    }

    public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        logger.LogTrace("Resolving {AssemblyName} dependency", assemblyName.Name);

        if (ResolveAssemblyFromOfflineNuGetAsync(assemblyName, CancellationToken.None).GetAwaiter().GetResult() is { } offlineResult)
        {
            var updatedIdentity = CheckAndDownloadUpdateAsync(assemblyName, offlineResult.Identity, CancellationToken.None).GetAwaiter().GetResult();

            if (updatedIdentity is not null && updatedIdentity.Version.CompareTo(offlineResult.Identity.Version) > 0)
            {
                var updatedAssemblyPath = ResolveAssemblyPathFromIdentityAsync(assemblyName, updatedIdentity, CancellationToken.None).GetAwaiter().GetResult();

                if (updatedAssemblyPath is not null)
                {
                    var assembly = context.LoadFromAssemblyPath(updatedAssemblyPath);

                    if (AreEqual(assembly.GetName(), assemblyName))
                        return assembly;

                    logger.LogError("Loaded assembly name mismatch for updated package {PackageId} version {Version}: expected {ExpectedName} but got {ActualName}, falling back to cached version", updatedIdentity.Id, updatedIdentity.Version, assemblyName.Name, assembly.GetName().Name);
                }
                else
                {
                    logger.LogError("Failed to resolve updated assembly path for {PackageId} version {Version}, falling back to cached version", updatedIdentity.Id, updatedIdentity.Version);
                }
            }

            if (System.IO.File.Exists(offlineResult.AssemblyPath))
            {
                var cachedAssembly = context.LoadFromAssemblyPath(offlineResult.AssemblyPath);

                if (AreEqual(cachedAssembly.GetName(), assemblyName))
                    return cachedAssembly;
            }
            else
            {
                logger.LogError("Cached assembly path {AssemblyPath} for {PackageId} no longer exists", offlineResult.AssemblyPath, offlineResult.Identity.Id);
            }
        }

        if (ResolveAssemblyFromOnlineNuGetAsync(assemblyName, CancellationToken.None).GetAwaiter().GetResult() is { } onlineAssemblyPath)
        {
            var assembly = context.LoadFromAssemblyPath(onlineAssemblyPath);

            if (AreEqual(assembly.GetName(), assemblyName))
                return assembly;
        }

        return null;

        static bool AreEqual(AssemblyName assemblyName1, AssemblyName assemblyName2) => assemblyName1.Name?.Equals(assemblyName2.Name) ?? false;
    }

    private async Task<(string AssemblyPath, PackageIdentity Identity)?> ResolveAssemblyFromOfflineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assemblyName.Name))
                return null;

            // TODO: directory name does not guarantee NuGet package name 
            var localPackagePath = Path.Combine(_packagesPath, assemblyName.Name.ToLower());

            if (!Directory.Exists(localPackagePath))
            {
                logger.LogTrace("Dependency {DependencyName} not found in the offline NuGet cache", assemblyName.Name);

                // some packages are easily resolved just by removing suffixes
                if (!assemblyName.Name.Contains('.'))
                    return null;

                var prefix = assemblyName.Name[..assemblyName.Name.LastIndexOf('.')];
                var variant = (AssemblyName)assemblyName.Clone();
                variant.Name = prefix;

                return await ResolveAssemblyFromOfflineNuGetAsync(variant, cancellationToken).ConfigureAwait(false);
            }

            var availableVersions = Directory.GetDirectories(localPackagePath).Select(Path.GetFileName).WhereNotNull().ToArray();

            if (availableVersions.Length == 0)
            {
                logger.LogWarning("No versions found for {DependencyName} in the offline NuGet cache", assemblyName.Name);
                return null;
            }

            var identities = availableVersions.Select(version => new PackageIdentity(assemblyName.Name, NuGetVersion.Parse(version)));
            var identity = SelectBestNuGetPackageVersion(identities, assemblyName.Version);

            if (identity == null)
            {
                logger.LogError("No matching version found for {DependencyName} in the offline NuGet cache", assemblyName.Name);
                return null;
            }

            var packagePath = Path.Combine(_packagesPath, identity.Id.ToLower(), identity.Version.ToString());
            using var packageReader = new PackageFolderReader(packagePath);

            var assemblyPath = await ResolveAssemblyPathFromPackageAsync(packageReader, packagePath, assemblyName, identity.Id, identity.Version, cancellationToken).ConfigureAwait(false);

            if (assemblyPath is null)
                throw new FileNotFoundException($"Dependency {assemblyName.Name} was found in the offline NuGet cache but the file cannot be located");

            return (assemblyPath, identity);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to resolve {DependencyName} from the offline NuGet cache", assemblyName.Name);
        }

        return null;
    }

    private async Task<PackageIdentity?> CheckAndDownloadUpdateAsync(AssemblyName assemblyName, PackageIdentity currentIdentity, CancellationToken cancellationToken = default)
    {
        PackageIdentity? bestUpdate = null;
        SourceRepository? bestRepository = null;

        foreach (var repository in Repositories)
        {
            try
            {
                var packages = await GetNuGetPackageVersionAsync(repository, currentIdentity.Id, cancellationToken).ConfigureAwait(false);
                var onlineIdentity = SelectBestNuGetPackageVersion(packages.Select(package => package.Identity), null);

                if (onlineIdentity is null)
                {
                    logger.LogTrace("Dependency {PackageId} not found in {Repository} for update check", currentIdentity.Id, repository.PackageSource.Name);
                    continue;
                }

                if (onlineIdentity.Version.CompareTo(currentIdentity.Version) > 0)
                {
                    if (bestUpdate is null || onlineIdentity.Version.CompareTo(bestUpdate.Version) > 0)
                    {
                        bestUpdate = onlineIdentity;
                        bestRepository = repository;
                    }
                }
                else
                {
                    logger.LogTrace("No update available for {PackageId} (current: {CurrentVersion}, online: {OnlineVersion})", currentIdentity.Id, currentIdentity.Version, onlineIdentity.Version);
                }
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Failed to check for updates for {PackageId} from {RepositoryName}", currentIdentity.Id, repository.PackageSource.Name);
            }
        }

        if (bestUpdate is not null && bestRepository is not null)
        {
            logger.LogInformation("Automatic update available for {PackageId}: {CurrentVersion} -> {NewVersion}. Downloading and applying update.", currentIdentity.Id, currentIdentity.Version, bestUpdate.Version);

            var packagePath = Path.Combine(_packagesPath, bestUpdate.Id.ToLower(), bestUpdate.Version.ToString());

            if (!Directory.Exists(packagePath))
            {
                await TryDownloadNuGetPackageAsync(bestRepository, bestUpdate, cancellationToken).ConfigureAwait(false);

                if (!Directory.Exists(packagePath))
                {
                    logger.LogError("Failed to download update for {PackageId} to version {NewVersion} from {RepositoryName}: package directory '{PackagePath}' does not exist after download attempt", bestUpdate.Id, bestUpdate.Version, bestRepository.PackageSource.Name, packagePath);
                    return null;
                }

                logger.LogInformation("Successfully updated {PackageId} to version {NewVersion}", bestUpdate.Id, bestUpdate.Version);
                return bestUpdate;
            }
            else
            {
                logger.LogTrace("Update for {PackageId} version {NewVersion} already exists in cache", bestUpdate.Id, bestUpdate.Version);
                return bestUpdate;
            }
        }

        return null;
    }

    private async Task<string?> ResolveAssemblyPathFromIdentityAsync(AssemblyName assemblyName, PackageIdentity identity, CancellationToken cancellationToken = default)
    {
        try
        {
            var packagePath = Path.Combine(_packagesPath, identity.Id.ToLower(), identity.Version.ToString());

            if (!Directory.Exists(packagePath))
            {
                logger.LogWarning("Package directory for {PackageId} version {Version} does not exist", identity.Id, identity.Version);
                return null;
            }

            using var packageReader = new PackageFolderReader(packagePath);

            return await ResolveAssemblyPathFromPackageAsync(packageReader, packagePath, assemblyName, identity.Id, identity.Version, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to resolve assembly path for {PackageId} version {Version}", identity.Id, identity.Version);
        }

        return null;
    }

    private async Task<string?> ResolveAssemblyPathFromPackageAsync(PackageFolderReader packageReader, string packagePath, AssemblyName assemblyName, string packageId, NuGetVersion packageVersion, CancellationToken cancellationToken)
    {
        var frameworks = await packageReader.GetLibItemsAsync(cancellationToken).ConfigureAwait(false);
        var targetFramework = NuGetFramework.ParseFrameworkName(FrameworkName, new DefaultFrameworkNameProvider());

        var compatibleFrameworks = frameworks
            .Where(f => DefaultCompatibilityProvider.Instance.IsCompatible(targetFramework, f.TargetFramework))
            .ToList();

        if (compatibleFrameworks.Count == 0)
        {
            logger.LogWarning("No compatible framework found for target framework {TargetFramework} in package {PackageId} version {Version}", targetFramework, packageId, packageVersion);
            return null;
        }

        foreach (var framework in compatibleFrameworks)
        {
            var assembly = framework.Items.FirstOrDefault(fileName => Path.GetFileName(fileName).Equals(assemblyName.Name + ".dll", StringComparison.InvariantCultureIgnoreCase));

            if (assembly is null)
            {
                assembly = framework.Items.FirstOrDefault();

                if (assembly is not null)
                {
                    logger.LogWarning("Using fallback assembly {FallbackAssembly} from framework {Framework} for requested assembly {RequestedAssembly} in package {PackageId} version {Version}", Path.GetFileName(assembly), framework.TargetFramework, assemblyName.Name, packageId, packageVersion);
                }
            }

            if (assembly is null)
            {
                continue;
            }

            return Path.Combine(packagePath, assembly);
        }

        logger.LogWarning("Assembly {AssemblyName} not found in any compatible framework for target framework {TargetFramework} in package {PackageId} version {Version}", assemblyName.Name, targetFramework, packageId, packageVersion);
        return null;
    }

    private async Task<string?> ResolveAssemblyFromOnlineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken = default)
    {
        foreach (var repository in Repositories)
        {
            try
            {
                var identity = await TryResolveNuGetIdentityAsync(repository, assemblyName, cancellationToken).ConfigureAwait(false);

                if (identity is null)
                {
                    logger.LogTrace("Dependency {DependencyName} not found in {Repository}", assemblyName.Name, repository.PackageSource.Name);
                    continue;
                }

                var packagePath = Path.Combine(_packagesPath, identity.Id.ToLower(), identity.Version.ToString());

                if (Directory.Exists(packagePath))
                    throw new InvalidOperationException($"Dependency {identity.Id} version {identity.Version} already exists in the NuGet cache. This should have been resolved in the offline step.");

                await TryDownloadNuGetPackageAsync(repository, identity, cancellationToken).ConfigureAwait(false);

                if (!Directory.Exists(packagePath))
                {
                    logger.LogWarning("Dependency {DependencyName} cannot be downloaded from {Repository}", identity.Id, repository.PackageSource.Name);
                    continue;
                }

                using var packageReader = new PackageFolderReader(packagePath);
                var frameworks = await packageReader.GetLibItemsAsync(cancellationToken).ConfigureAwait(false);
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
            catch (Exception exception)
            {
                logger.LogTrace(exception, "Dependency {PackageId} cannot be resolved from NuGet repository {RepositoryName}", assemblyName.Name, repository.PackageSource.Name);
            }
        }

        return null;
    }

    private async Task TryDownloadNuGetPackageAsync(SourceRepository repository, PackageIdentity identity, CancellationToken cancellationToken)
    {
        try
        {
            using var result = await PackageDownloader.GetDownloadResourceResultAsync(repository, identity, new PackageDownloadContext(Cache), _packagesPath, _nugetLogger, cancellationToken).ConfigureAwait(false);
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
        var identity = await TryResolveNuGetPackageIdAsync(repository, assemblyName, cancellationToken).ConfigureAwait(false);

        if (identity is not null)
            return identity;

        logger.LogTrace("Looking for dependency {DependencyName} with TryGet in NuGet", assemblyName.Name);
        identity = await TryResolveNuGetPackageSearchAsync(repository, assemblyName, cancellationToken).ConfigureAwait(false);

        if (identity is not null)
            return identity;

        logger.LogTrace("Dependency {DependencyName} not found in NuGet", assemblyName.Name);
        return null;
    }

    private async Task<PackageIdentity?> TryResolveNuGetPackageIdAsync(SourceRepository repository, AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(assemblyName.Name))
            return null;

        var packages = await GetNuGetPackageVersionAsync(repository, assemblyName.Name, cancellationToken).ConfigureAwait(false);
        var best = SelectBestNuGetPackageVersion(packages.Select(package => package.Identity), assemblyName.Version);

        return best;
    }

    private async Task<PackageIdentity?> TryResolveNuGetPackageSearchAsync(SourceRepository repository, AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        var packageSearchResource = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken).ConfigureAwait(false);
        var packageSearchResults = await packageSearchResource.SearchAsync(assemblyName.Name, new SearchFilter(true), 0, 1, _nugetLogger, cancellationToken).ConfigureAwait(false);

        // actually always 1
        foreach (var packageSearchResult in packageSearchResults)
        {
            var packages = await GetNuGetPackageVersionAsync(repository, packageSearchResult.Identity.Id, cancellationToken).ConfigureAwait(false);
            var best = SelectBestNuGetPackageVersion(packages.Select(package => package.Identity), assemblyName.Version);

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
        var packageMetadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken).ConfigureAwait(false);
        return await packageMetadataResource.GetMetadataAsync(packageId, true, false, Cache, _nugetLogger, cancellationToken).ConfigureAwait(false);
    }

    private async Task ProbeRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        if (!Repositories.Any())
            return;

        var statuses = new List<(string Url, string Status)>();

        foreach (var repository in UriRepositories)
        {
            var sanitizedUrl = repository.Contains('@') ? repository[(repository.IndexOf('@') + 1)..] : repository;

            if (!Uri.TryCreate(repository, UriKind.Absolute, out var uri))
            {
                logger.LogTrace("Invalid NuGet repository URI: {RepositoryUri}", sanitizedUrl);
                statuses.Add((sanitizedUrl, "Invalid"));
                continue;
            }

            try
            {
                using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
                using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, linkedCancellationTokenSource.Token).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogTrace("NuGet repository {RepositoryUrl} returned non-success status code: {StatusCode}", sanitizedUrl, response.StatusCode);
                    statuses.Add((sanitizedUrl, $"Http{(int)response.StatusCode}"));
                }
                else
                {
                    statuses.Add((sanitizedUrl, "Ok"));
                }
            }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw;

                logger.LogTrace("NuGet repository {RepositoryUrl} timed out", sanitizedUrl);
                statuses.Add((sanitizedUrl, "Timeout"));
            }
            catch (Exception exception)
            {
                logger.LogTrace("NuGet repository {RepositoryUrl} is not responding: {Message}", sanitizedUrl, exception.Message);
                statuses.Add((sanitizedUrl, "NotConnected"));
            }
        }

        if (statuses.Count > 0)
            logger.LogInformation("Custom NuGet repositories:");

        foreach (var (url, status) in statuses)
            logger.LogInformation(" - {Url} [{Status}]", url, status);
    }

    [GeneratedRegex(@"(?<!\\);")]
    private static partial Regex UnescapedSemicolonRegex();
}
