using System.Linq;
using Godot;
using Peytral.Scanners;
using Peytral.Scanners.Rules;

[GlobalClass]
public partial class AssemblyReferenceScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner()
  {
    return new AssemblyReferencesScanner()
    {
      ScannerName = ScannerNameOverride,
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
