using System;
using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class LogPrefab : PanelContainer
{
  [Export]
  public Label SenderLabel;
  [Export]
  public RichTextLabel MessageContent;

  public void DisplayLog(LogEventArgs log)
  {
    string color = log.Level switch
    {
      SeverityLevel.Alert or SeverityLevel.Critical => "[color=orange]",
      SeverityLevel.Warning => "[color=yellow]",
      _ => "[color=lightblue]",
    };
    SenderLabel.Text = $"[{log.Sender}]";
    MessageContent.Text = $"{color}{log.Message}[/color]";
  }
}
