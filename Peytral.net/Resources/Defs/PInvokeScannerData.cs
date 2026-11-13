using System.Linq;
using AssemblyInspect.Scanners;
using AssemblyInspect.Scanners.Rules;
using Godot;

[GlobalClass]
public partial class PInvokeScannerData : ScannerData
{
  public override IAssemblyScanner BuildScanner()
  {
    return new PInvokeScanner()
    {
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
