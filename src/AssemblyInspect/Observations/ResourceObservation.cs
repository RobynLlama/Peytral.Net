namespace AssemblyInspect.Observations;

public record ResourceObservation(
    string ScannerName,
    SeverityLevel Severity,
    string ResourceName,
    string Summary,
    string Analysis,
    byte[] ResourceData
) : Observation(ScannerName, Severity, Summary, Analysis, ResourceName);
