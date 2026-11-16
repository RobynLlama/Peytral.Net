using System.Linq;
using AssemblyInspect.Scanners;
using AssemblyInspect.Scanners.Rules;
using Godot;

[GlobalClass]
public partial class SourceBlobScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner()
  {
    return new SourceBlobScanner()
    {
      ScannerName = ScannerNameOverride,
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
