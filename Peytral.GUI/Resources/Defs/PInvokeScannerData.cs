using System.Linq;
using Godot;
using Peytral.Scanners;
using Peytral.Scanners.Rules;

[GlobalClass]
public partial class PInvokeScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner()
  {
    return new PInvokeScanner()
    {
      ScannerName = ScannerNameOverride,
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
