using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners.Rules;

public record PlainTextRule(string Pattern, string Summary, string Description, SeverityLevel Severity);
