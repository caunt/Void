﻿using System.Reflection;
using System.Runtime.Versioning;
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
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins;

public class PluginDependencyService(ILogger<PluginDependencyService> logger) : IPluginDependencyService
{
    private static readonly string FrameworkName = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? throw new InvalidOperationException("Cannot determine the target framework.");
    private static readonly SourceRepository NuGetRepository = Repository.Factory.GetCoreV3(new PackageSource("https://api.nuget.org/v3/index.json").Source);
    private static readonly SourceCacheContext NuGetCache = new();
    private static readonly string NuGetPackagesPath = Path.Combine(Directory.GetCurrentDirectory(), SettingsUtility.DefaultGlobalPackagesFolderPath);

    public string? ResolveAssemblyPath(AssemblyName assemblyName)
    {
        logger.LogInformation("Resolving {AssemblyName} dependency", assemblyName.Name);

        if (assemblyName.FullName.StartsWith(nameof(Void) + '.'))
        {
            logger.LogCritical("Void packages shouldn't be searched in NuGet");
            return null;
        }

        var assemblyPath = ResolveAssemblyFromNuGetAsync(assemblyName).GetAwaiter().GetResult();
        return assemblyPath;
    }

    private async ValueTask<string?> ResolveAssemblyFromNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken = default)
    {
        var assemblyPath = await ResolveAssemblyFromOfflineNuGetAsync(assemblyName, cancellationToken);
        
        if (string.IsNullOrWhiteSpace(assemblyPath))
            assemblyPath = await ResolveAssemblyFromOnlineNuGetAsync(assemblyName, cancellationToken);

        return assemblyPath;
    }

    private async ValueTask<string?> ResolveAssemblyFromOfflineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assemblyName.Name))
                return null;

            // TODO: directory name does not guarantee NuGet package name 
            var localPackagePath = Path.Combine(NuGetPackagesPath, assemblyName.Name.ToLower());

            if (!Directory.Exists(localPackagePath))
            {
                logger.LogWarning("Dependency {DependencyName} not found in the offline NuGet cache", assemblyName.Name);

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

                if (assembly is null)
                    throw new FileNotFoundException($"Dependency {assemblyName.Name} was found in the offline NuGet cache but the file cannot be located");

                return Path.Combine(packagePath, assembly);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resolve {DependencyName} from the offline NuGet cache", assemblyName.Name);
        }

        return null;
    }

    private async ValueTask<string?> ResolveAssemblyFromOnlineNuGetAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        try
        {
            var identity = await TryResolveNuGetIdentityAsync(assemblyName, cancellationToken);

            if (identity is null)
            {
                logger.LogWarning("Dependency {DependencyName} not found in NuGet at all", assemblyName.Name);
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

                if (assembly is null)
                    throw new FileNotFoundException($"Dependency {identity.Id} was downloaded from NuGet but file cannot be located");

                return Path.Combine(packagePath, assembly);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Dependency {PackageId} cannot be resolved from NuGet", assemblyName.Name);
        }

        return null;
    }

    private async ValueTask TryDownloadNuGetPackageAsync(PackageIdentity identity, CancellationToken cancellationToken)
    {
        try
        {
            using var result = await PackageDownloader.GetDownloadResourceResultAsync(NuGetRepository, identity, new PackageDownloadContext(NuGetCache), NuGetPackagesPath, NullLogger.Instance, cancellationToken);
            logger.LogInformation("Downloaded {PackageId} {PackageVersion}", identity.Id, identity.Version);
        }
        catch (FatalProtocolException exception)
        {
            logger.LogCritical("Dependency {PackageId} cannot be downloaded: {Reason}", identity.Id, exception.Message);
        }
        catch (RetriableProtocolException exception)
        {
            logger.LogError("Dependency {PackageId} loading was cancelled: {Message}", identity.Id, exception.Message);
        }
    }

    private async ValueTask<PackageIdentity?> TryResolveNuGetIdentityAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Looking for dependency {DependencyName} as Identity in NuGet", assemblyName.Name);
        var identity = await TryResolveNuGetPackageIdAsync(assemblyName, cancellationToken);

        if (identity is not null)
            return identity;

        logger.LogInformation("Looking for dependency {DependencyName} with Search in NuGet", assemblyName.Name);
        identity = await TryResolveNuGetPackageSearchAsync(assemblyName, cancellationToken);

        if (identity is not null)
            return identity;

        logger.LogWarning("Dependency {DependencyName} not found in NuGet", assemblyName.Name);
        return null;
    }

    private async ValueTask<PackageIdentity?> TryResolveNuGetPackageIdAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(assemblyName.Name))
            return null;

        var packages = await GetNuGetPackageVersionAsync(assemblyName.Name, cancellationToken);
        var best = SelectBestNuGetPackageVersion(packages.Select(package => package.Identity), assemblyName.Version);

        return best;
    }

    private async ValueTask<PackageIdentity?> TryResolveNuGetPackageSearchAsync(AssemblyName assemblyName, CancellationToken cancellationToken)
    {
        var packageSearchResource = await NuGetRepository.GetResourceAsync<PackageSearchResource>(cancellationToken);
        var packageSearchResults = await packageSearchResource.SearchAsync(assemblyName.Name, new SearchFilter(true), 0, 1, NullLogger.Instance, cancellationToken);

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

        logger.LogInformation("Dependency {DependencyName} resolved with version {DependencyVersion}", result.Id, result.Version);
        return result;
    }

    private static async ValueTask<IEnumerable<IPackageSearchMetadata>> GetNuGetPackageVersionAsync(string packageId, CancellationToken cancellationToken)
    {
        var packageMetadataResource = await NuGetRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await packageMetadataResource.GetMetadataAsync(packageId, true, false, NuGetCache, NullLogger.Instance, cancellationToken);
    }
}