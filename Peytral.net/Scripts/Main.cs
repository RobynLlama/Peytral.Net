using System;
using AssemblyInspect;
using Godot;
using Peytral;

public partial class Main : PanelContainer
{
  [Export]
  public FileDialog Selector;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    Selector.FileSelected += FileSelected;
  }

  private void FileSelected(string path) =>
    DocumentManager.OpenNewDocument(path);
}
