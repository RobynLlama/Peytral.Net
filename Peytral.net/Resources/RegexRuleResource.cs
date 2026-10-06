using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public partial class RegexRuleResource : Resource
{
  [Export]
  public string Pattern;
  [Export]
  public string Summary;
  [Export]
  public string Description;
  [Export]
  public SeverityLevel Severity;

  public RegexRule AsRule() =>
    new(Pattern, Summary, Description, Severity);

  public static implicit operator RegexRule(RegexRuleResource self) => self.AsRule();
}
