using System.Linq;
using AssemblyInspect.Scanners;
using AssemblyInspect.Scanners.Rules;
using Godot;

[GlobalClass]
public partial class SourceBlobScannerData : ScannerData
{

  [Export]
  public RegexRuleResource[] RegexRules;
  [Export]
  public PlainTextRuleResource[] PlainTextRules;

  public override IAssemblyScanner BuildScanner()
  {
    return new SourceBlobScanner()
    {
      RegexRules = RegexRules.Select(x => (RegexRule)x),
      PlainTextRules = PlainTextRules.Select(x => (PlainTextRule)x),
    };
  }
}
