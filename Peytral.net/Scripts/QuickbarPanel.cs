using System;
using System.ComponentModel;
using AssemblyInspect;
using Godot;

[GlobalClass]
public partial class QuickbarPanel : PanelContainer
{
  [Export, ExportGroup("Buttons")]
  public Button BtnHide;
  [Export]
  public Button BtnNew;
  [Export]
  public Button BtnSave;
  [Export]
  public Button BtnClose;
  [Export, ExportGroup("Other")]
  public Control HidePanel;
  [Export]
  public FileDialog Selector;
  public override void _Ready()
  {
    BtnHide.Pressed += HideToggle;
    BtnNew.Pressed += NewDocument;
  }

  private void HideToggle() =>
    HidePanel.Visible = !HidePanel.Visible;

  private void NewDocument()
  {
    Selector.PopupCentered();
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }
}
