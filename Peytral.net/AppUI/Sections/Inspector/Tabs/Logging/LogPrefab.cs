using System;
using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class LogPrefab : PanelContainer
{
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
    MessageContent.Text = $"[{log.Sender}]{color} {log.Message}[/color]";
  }
}
