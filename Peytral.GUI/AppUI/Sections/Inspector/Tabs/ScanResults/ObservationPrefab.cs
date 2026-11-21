using Godot;
using Peytral.Observations;

[GlobalClass]
public partial class ObservationPrefab : PanelContainer, IPoolItem
{
  [Export, ExportGroup("Text")]
  public Label SummaryLabel;
  [Export]
  public Label ContentLabel;
  [Export]
  public Label ScannerNameLabel;
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
    ScannerNameLabel.Text = $"Detected by: {obs.ScannerName}";
    LocationLabel.Text = $"Detected in: {obs.Location}";

    AlertIcon.Visible = obs.Severity == SeverityLevel.Alert || obs.Severity == SeverityLevel.Critical;
    WarnIcon.Visible = obs.Severity == SeverityLevel.Warning;
    MsgIcon.Visible = obs.Severity == SeverityLevel.Message;
  }

  public void EnterPool() =>
    Visible = false;

  public void ExitPool() { }
}
