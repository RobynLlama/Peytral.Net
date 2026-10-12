using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class ResourceScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner()
  {
    return new ResourceScanner();
  }
}
