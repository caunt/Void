using Nito.Disposables.Internals;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Reflection;
using System.Runtime.Versioning;

namespace Void.Proxy.Plugins.Dependencies.Remote.NuGetSource;

public class NuGetDependencyResolver(ILogger logger)
{
    private static readonly string FrameworkName = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
        ?? throw new InvalidOperationException("Cannot determine the target framework.");

    private static readonly SourceRepository NuGetRepository = Repository.Factory.GetCoreV3(new PackageSource("https://api.nuget.org/v3/index.json").Source);
    private static readonly SourceCacheContext NuGetCache = new();
    private static readonly string NuGetPackagesPath = Path.Combine(Directory.GetCurrentDirectory(), SettingsUtility.DefaultGlobalPackagesFolderPath);

    private readonly NuGet.Common.ILogger _nugetLogger = new NuGetLogger(logger);

    private async Task<string?> ResolveAssemblyFromNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken = default)
    {
        var assemblyPath = await ResolveAssemblyFromOfflineNuGetAsync(assemblyName, cancellationToken);

        if (string.IsNullOrWhiteSpace(assemblyPath))
            assemblyPath = await ResolveAssemblyFromOnlineNuGetAsync(assemblyName, cancellationToken);

        return assemblyPath;
    }

    public string? ResolveAssemblyPath(AssemblyName assemblyName)
    {
        logger.LogTrace("Resolving {AssemblyName} dependency", assemblyName.Name);

        if (assemblyName.FullName.StartsWith(nameof(Void) + '.'))
        {
            logger.LogCritical("Void package {AssemblyName} shouldn't be searched in NuGet", assemblyName.Name);
            return null;
        }

        var assemblyPath = ResolveAssemblyFromNuGetAsync(assemblyName).GetAwaiter().GetResult();
        return assemblyPath;
    }

    private async Task<string?> ResolveAssemblyFromOfflineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assemblyName.Name))
                return null;

            // TODO: directory name does not guarantee NuGet package name 
            var localPackagePath = Path.Combine(NuGetPackagesPath, assemblyName.Name.ToLower());

            if (!Directory.Exists(localPackagePath))
            {
                logger.LogTrace("Dependency {DependencyName} not found in the offline NuGet cache", assemblyName.Name);

                // some packages easily resolved with just removed suffixes
                if (!assemblyName.Name.Contains('.'))
                    return null;

                var prefix = assemblyName.Name[..assemblyName.Name.LastIndexOf('.')];
                assemblyName.Name = prefix;

                return await ResolveAssemblyFromOfflineNuGetAsync(assemblyName, cancellationToken);
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

            var packagePath = Path.Combine(NuGetPackagesPath, identity.Id.ToLower(), identity.Version.ToString());
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

    private async Task<string?> ResolveAssemblyFromOnlineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        try
        {
            var identity = await TryResolveNuGetIdentityAsync(assemblyName, cancellationToken);

            if (identity is null)
            {
                logger.LogTrace("Dependency {DependencyName} not found in NuGet at all", assemblyName.Name);
                return null;
            }

            var packagePath = Path.Combine(NuGetPackagesPath, identity.Id.ToLower(), identity.Version.ToString());

            if (!Directory.Exists(packagePath))
                await TryDownloadNuGetPackageAsync(identity, cancellationToken);

            if (!Directory.Exists(packagePath))
            {
                logger.LogWarning("Dependency {DependencyName} cannot be downloaded from NuGet", identity.Id);
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
        catch (Exception exception)
        {
            logger.LogError(exception, "Dependency {PackageId} cannot be resolved from NuGet", assemblyName.Name);
        }

        return null;
    }

    private async Task TryDownloadNuGetPackageAsync(PackageIdentity identity, CancellationToken cancellationToken)
    {
        try
        {
            using var result = await PackageDownloader.GetDownloadResourceResultAsync(NuGetRepository, identity, new PackageDownloadContext(NuGetCache), NuGetPackagesPath, _nugetLogger, cancellationToken);
            logger.LogTrace("Downloaded {PackageId} version {PackageVersion} dependency", identity.Id, identity.Version);
        }
        catch (FatalProtocolException exception)
        {
            logger.LogError("Dependency {PackageId} cannot be downloaded: {Reason}", identity.Id, exception.Message);
        }
        catch (RetriableProtocolException exception)
        {
            logger.LogWarning("Dependency {PackageId} loading was cancelled: {Message}", identity.Id, exception.Message);
        }
    }

    private async Task<PackageIdentity?> TryResolveNuGetIdentityAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        logger.LogTrace("Looking for dependency {DependencyName} as Identity in NuGet", assemblyName.Name);
        var identity = await TryResolveNuGetPackageIdAsync(assemblyName, cancellationToken);

        if (identity is not null)
            return identity;

        logger.LogTrace("Looking for dependency {DependencyName} with TryGet in NuGet", assemblyName.Name);
        identity = await TryResolveNuGetPackageSearchAsync(assemblyName, cancellationToken);

        if (identity is not null)
            return identity;

        logger.LogTrace("Dependency {DependencyName} not found in NuGet", assemblyName.Name);
        return null;
    }

    private async Task<PackageIdentity?> TryResolveNuGetPackageIdAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(assemblyName.Name))
            return null;

        var packages = await GetNuGetPackageVersionAsync(assemblyName.Name, cancellationToken);
        var best = SelectBestNuGetPackageVersion(packages.Select(package => package.Identity), assemblyName.Version);

        return best;
    }

    private async Task<PackageIdentity?> TryResolveNuGetPackageSearchAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        var packageSearchResource = await NuGetRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);
        var packageSearchResults = await packageSearchResource.SearchAsync(assemblyName.Name, new SearchFilter(true), 0, 1, _nugetLogger, cancellationToken);

        // actually always 1
        foreach (var packageSearchResult in packageSearchResults)
        {
            var packages = await GetNuGetPackageVersionAsync(packageSearchResult.Identity.Id, cancellationToken);
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

            if (identity.Version.CompareTo(identity.Version) < 0)
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

    private async Task<IEnumerable<IPackageSearchMetadata>> GetNuGetPackageVersionAsync(string packageId, CancellationToken cancellationToken)
    {
        var packageMetadataResource = await NuGetRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await packageMetadataResource.GetMetadataAsync(packageId, true, false, NuGetCache, _nugetLogger, cancellationToken);
    }
}
