// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    category: "MicrosoftCodeAnalysisCorrectness",
    checkId: "RS1041:Compiler extensions should be implemented in assemblies targeting netstandard2.0",
    Justification = "Suppression is necessary for this project.",
    Scope = "type",
    Target = "~T:Void.IntegrationTests.Generators.DirectConnectionReleaseGroupGenerator")]
