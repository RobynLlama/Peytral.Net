using System.Linq;
using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class PInvokeScannerData : ScannerData
{

  [Export]
  public RegexRuleResource[] RegexRules;
  [Export]
  public PlainTextRuleResource[] PlainTextRules;

  public override IAssemblyScanner BuildScanner()
  {
    return new PInvokeScanner()
    {
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
