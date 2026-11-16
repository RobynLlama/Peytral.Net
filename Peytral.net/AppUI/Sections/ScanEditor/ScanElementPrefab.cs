using System;
using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class ScanElementPrefab : MarginContainer, IPoolItem
{
  public bool IsRegex = false;

  [Export]
  public Label LblTitle;
  [Export]
  public Button BtnDelete;

  [Export, ExportGroup("Inputs")]
  public LineEdit SummaryInput;
  [Export]
  public LineEdit PatternInput;
  [Export]
  public LineEdit MessageInput;
  [Export]
  public OptionButton SeverityInput;
  public event Action<ScanElementPrefab> OnRemovePressed;

  public override void _Ready()
  {
    VisibilityChanged += UpdateTitle;
    BtnDelete.Pressed += () => OnRemovePressed?.Invoke(this);
  }

  private void UpdateTitle()
  {
    if (!Visible)
      return;

    LblTitle.Text = IsRegex ? "Regex Element" : "PlainText Element";
  }

  public void EnterPool()
  {
    Visible = false;
    SummaryInput.Clear();
    PatternInput.Clear();
    MessageInput.Clear();
    SeverityInput.Selected = 0;
  }

  public void ExitPool() { }

  public PlainTextRuleResource CreatePlainTextResource()
  {
    return new PlainTextRuleResource()
    {
      Summary = SummaryInput.Text,
      Description = MessageInput.Text,
      Pattern = PatternInput.Text,
      Severity = (SeverityLevel)SeverityInput.Selected,
    };
  }

  public RegexRuleResource CreateRegexResource()
  {
    return new RegexRuleResource()
    {
      Summary = SummaryInput.Text,
      Description = MessageInput.Text,
      Pattern = PatternInput.Text,
      Severity = (SeverityLevel)SeverityInput.Selected,
    };
  }
}
