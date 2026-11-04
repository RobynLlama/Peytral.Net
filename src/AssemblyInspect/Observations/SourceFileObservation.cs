namespace AssemblyInspect.Observations;

public record SourceFileObservation(
  string ScannerName,
  SeverityLevel Severity,
  string Summary,
  string Description,
  string FilePath,
  int LineNumber
) : Observation(ScannerName, Severity, Summary, Description, $"{FilePath} L{LineNumber}");
