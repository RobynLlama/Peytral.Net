using System.Collections.Generic;
using System.Linq;
using Godot;
using Peytral.Scanners;

[GlobalClass]
public partial class ScannerPackage : Resource
{
  [Export]
  public ScannerData[] Scanners;

  public IEnumerable<IAssemblyScanner> BuildScanners =>
    Scanners.Select(x => x.BuildScanner());
}
