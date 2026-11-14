namespace AssemblyInspect.Scanners.Models;

public partial class ScanContext
{
  public record ScanConfiguration(
    string VTreeRootOverride = "/"
    );
  public readonly ScanConfiguration Config;
}
