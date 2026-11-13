using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public abstract partial class ScannerData : Resource
{
  [Export]
  public RegexRuleResource[] RegexRules;
  [Export]
  public PlainTextRuleResource[] PlainTextRules;
  public abstract IAssemblyScanner BuildScanner();
}
