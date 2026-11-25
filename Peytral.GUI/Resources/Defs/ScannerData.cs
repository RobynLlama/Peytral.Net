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
  [Export]
  public string ScannerEditorDescription = "No description given";
  public abstract IAssemblyScanner BuildScanner();
}
