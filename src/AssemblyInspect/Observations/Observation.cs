namespace AssemblyInspect.Observations;

/// <summary>
/// Represents a single heuristic finding, insight, or potential security threat detected within an assembly by a scanner.
/// </summary>
/// <param name="ScannerName">The name or identifier of the scanner rule that flagged this observation.</param>
/// <param name="Severity">The threat or importance level assigned to this observation, ranging from informational messages to critical threats.</param>
/// <param name="Summary">A brief, single-line headline summarizing what was detected (e.g., for list views or CLI table outputs).</param>
/// <param name="Description">A detailed explanation of why the pattern was flagged, its potential risks, and context for manual inspection.</param>
/// <param name="Location">A human-readable description of where the hit occurred within the assembly (e.g., type name, method signature, or IL offset).</param>
public record Observation(
    string ScannerName,
    SeverityLevel Severity,
    string Summary,
    string Description,
    string Location
);
