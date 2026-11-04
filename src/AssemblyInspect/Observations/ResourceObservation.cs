namespace AssemblyInspect.Observations;

public record ResourceObservation(
  string ScannerName,
  SeverityLevel Severity,
  string ResourceName,
  string Summary,
  string Description,
  byte[] ResourceData
) : Observation(ScannerName, Severity, Summary, Description, ResourceName);
