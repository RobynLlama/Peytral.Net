using Godot;
using Peytral.Scanners;

[GlobalClass]
public partial class ResourceScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner() =>
   new ResourceScanner() { ScannerName = ScannerNameOverride, };
}
