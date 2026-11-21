using System;
using System.Collections.Generic;
using System.IO;
using Godot;

public partial class LoggingTab : MarginContainer
{
  [Export]
  public Button BtnOpenLogs;
  [Export]
  public PackedScene Prefab;
  [Export]
  public VBoxContainer ItemContainer;
  private readonly Queue<LogPrefab> LogItems = [];

  public override void _Ready()
  {
    BtnOpenLogs.Pressed += () => OS.ShellOpen(Path.Combine(OS.GetUserDataDir(), "logs"));
    for (int i = 0; i < 100; i++)
    {
      var child = Prefab.Instantiate<LogPrefab>();
      child.Visible = false;
      ItemContainer.AddChild(child);
      LogItems.Enqueue(child);
    }
    Logger.OnReceivedLog += HandleLog;
  }

  private void HandleLog(LogEventArgs args)
  {
    var entry = NextChild();
    entry.DisplayLog(args);
    ItemContainer.MoveChild(entry, -1);
    entry.Visible = true;
  }

  private LogPrefab NextChild()
  {
    var child = LogItems.Dequeue();
    LogItems.Enqueue(child);
    return child;
  }
}
