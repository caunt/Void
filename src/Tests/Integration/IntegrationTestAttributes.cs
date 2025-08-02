using System;
using Xunit;

namespace Void.Tests.Integration;

internal static class IntegrationTestEnvironment
{
    public const string DirectTestsEnabledVariable = "VOID_INTEGRATION_DIRECT_TESTS_ENABLED";
    public const string ProxiedTestsEnabledVariable = "VOID_INTEGRATION_PROXIED_TESTS_ENABLED";

    public static bool DirectTestsEnabled { get; } = bool.TryParse(Environment.GetEnvironmentVariable(DirectTestsEnabledVariable), out var enabled) && enabled;
    public static bool ProxiedTestsEnabled { get; } = bool.TryParse(Environment.GetEnvironmentVariable(ProxiedTestsEnabledVariable), out var enabled) && enabled;
}

public sealed class DirectFactAttribute : FactAttribute
{
    public DirectFactAttribute()
    {
        if (!IntegrationTestEnvironment.DirectTestsEnabled)
            Skip = $"Direct integration tests are disabled. Set {IntegrationTestEnvironment.DirectTestsEnabledVariable}=true to enable.";
    }
}

public sealed class DirectTheoryAttribute : TheoryAttribute
{
    public DirectTheoryAttribute()
    {
        if (!IntegrationTestEnvironment.DirectTestsEnabled)
            Skip = $"Direct integration tests are disabled. Set {IntegrationTestEnvironment.DirectTestsEnabledVariable}=true to enable.";
    }
}

public sealed class ProxiedFactAttribute : FactAttribute
{
    public ProxiedFactAttribute()
    {
        if (!IntegrationTestEnvironment.ProxiedTestsEnabled)
            Skip = $"Proxied integration tests are disabled. Set {IntegrationTestEnvironment.ProxiedTestsEnabledVariable}=true to enable.";
    }
}

public sealed class ProxiedTheoryAttribute : TheoryAttribute
{
    public ProxiedTheoryAttribute()
    {
        if (!IntegrationTestEnvironment.ProxiedTestsEnabled)
            Skip = $"Proxied integration tests are disabled. Set {IntegrationTestEnvironment.ProxiedTestsEnabledVariable}=true to enable.";
    }
}

