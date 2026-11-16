using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class ScannerPackage : Resource
{
  [Export]
  ScannerData[] Scanners;

  public IEnumerable<IAssemblyScanner> BuildScanners =>
    Scanners.Select(x => x.BuildScanner());
}
