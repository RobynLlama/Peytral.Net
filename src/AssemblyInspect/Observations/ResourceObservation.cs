using ICSharpCode.Decompiler.Metadata;

namespace AssemblyInspect.Observations;

public record ResourceObservation(
  string ScannerName,
  SeverityLevel Severity,
  string ResourceName,
  string ResourceLocation,
  string Summary,
  string Description,
  byte[] ResourceData
) : Observation(ScannerName, Severity, Summary, Description, $"{ResourceLocation} → {ResourceName}");
