using System;
using System.Collections.Generic;
using System.IO;
using Godot;

[GlobalClass]
public partial class LoggingTab : PanelContainer
{
  [Export]
  public Button BtnOpenLogs;
  [Export]
  public PackedScene Prefab;
  [Export]
  public VBoxContainer ItemContainer;
  [Export]
  public ScrollContainer Scroller;
  [Export]
  public CheckButton BtnAutoScroll;
  private readonly Queue<LogPrefab> LogItems = [];
  private bool ScrollNeedsUpdate = false;

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
    Scroller.GetVScrollBar().Changed += ScrollView;
  }

  private void ScrollView()
  {
    if (ScrollNeedsUpdate)
    {
      Scroller.GetVScrollBar().Value = int.MaxValue;
      ScrollNeedsUpdate = false;
    }
  }

  private void HandleLog(LogEventArgs args)
  {
    var entry = NextChild();
    entry.DisplayLog(args);
    ItemContainer.MoveChild(entry, -1);
    entry.Visible = true;

    if (BtnAutoScroll.ButtonPressed)
      ScrollNeedsUpdate = true;
  }

  private LogPrefab NextChild()
  {
    var child = LogItems.Dequeue();
    LogItems.Enqueue(child);
    return child;
  }
}
