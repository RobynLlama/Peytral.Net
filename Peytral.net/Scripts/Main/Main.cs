using System;
using AssemblyInspect;
using Godot;
using Peytral;

public partial class Main : PanelContainer
{
  [Export]
  public FileDialog Selector;

  [Export]
  public Control WorkingPanel;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    Selector.FileSelected += FileSelected;
  }

  private async void FileSelected(string path)
  {
    WorkingPanel.Visible = true;
    await DocumentManager.OpenNewDocument(path);
    WorkingPanel.Visible = false;
  }

}
