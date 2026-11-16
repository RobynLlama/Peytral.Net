using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class ResourceScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner() =>
   new ResourceScanner() { ScannerName = ScannerNameOverride, };
}
