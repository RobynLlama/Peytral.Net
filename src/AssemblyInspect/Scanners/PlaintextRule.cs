using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public record PlainTextRule(string Pattern, string Summary, string Description, SeverityLevel Severity);
