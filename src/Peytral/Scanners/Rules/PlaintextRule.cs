using Peytral.Observations;

namespace Peytral.Scanners.Rules;

public record PlainTextRule(string Pattern, string Summary, string Description, SeverityLevel Severity);
