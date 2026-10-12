using System.Linq;
using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class AssemblyReferenceScannerData : ScannerData
{

  [Export]
  public RegexRuleResource[] RegexRules;
  [Export]
  public PlainTextRuleResource[] PlainTextRules;

  public override IAssemblyScanner BuildScanner()
  {
    return new AssemblyReferencesScanner()
    {
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
