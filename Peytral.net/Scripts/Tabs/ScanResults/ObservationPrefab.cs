using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class ObservationPrefab : PanelContainer
{
  [Export, ExportGroup("Text")]
  public Label SummaryLabel;
  [Export]
  public Label ContentLabel;
  [Export]
  public Label LocationLabel;
  [Export, ExportGroup("Images")]
  public TextureRect AlertIcon;
  [Export]
  public TextureRect WarnIcon;
  [Export]
  public TextureRect MsgIcon;

  public void DisplayObservation(Observation obs)
  {
    SummaryLabel.Text = obs.Summary;
    ContentLabel.Text = obs.Description;
    LocationLabel.Text = $"Detected in: {obs.Location}";

    AlertIcon.Visible = obs.Severity == SeverityLevel.Alert || obs.Severity == SeverityLevel.Critical;
    WarnIcon.Visible = obs.Severity == SeverityLevel.Warning;
    MsgIcon.Visible = obs.Severity == SeverityLevel.Message;
  }
}
