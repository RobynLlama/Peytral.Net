using Godot;
using Peytral.Scanners;

[GlobalClass]
public abstract partial class ScannerData : Resource
{
  [Export]
  public RegexRuleResource[] RegexRules;
  [Export]
  public PlainTextRuleResource[] PlainTextRules;
  [Export]
  public string ScannerNameOverride;
  public abstract IAssemblyScanner BuildScanner();
}
