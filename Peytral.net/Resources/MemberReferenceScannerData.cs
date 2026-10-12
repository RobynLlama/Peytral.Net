using System.Linq;
using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class MemberReferenceScannerData : ScannerData
{

  [Export]
  public RegexRuleResource[] RegexRules;
  [Export]
  public PlainTextRuleResource[] PlainTextRules;

  public override IAssemblyScanner BuildScanner()
  {
    return new MemberReferenceScanner()
    {
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
