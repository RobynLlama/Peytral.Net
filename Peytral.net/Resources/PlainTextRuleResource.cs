using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class PlainTextRuleResource : Resource
{
  [Export]
  public string Pattern;
  [Export]
  public string Summary;
  [Export]
  public string Description;
  [Export]
  public SeverityLevel Severity;

  public PlainTextRule AsRule() =>
    new(Pattern, Summary, Description, Severity);

  public static implicit operator PlainTextRule(PlainTextRuleResource self) => self.AsRule();
}
