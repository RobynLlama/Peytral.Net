using System;
using Godot;

public partial class Main : PanelContainer
{
  [Export]
  public InspectorSection Inspector;
  [Export]
  public ScanEditorSection ScanEditor;
  public override void _Ready()
  {
    Inspector.BtnSwitch.Pressed += SwitchToEditor;
    ScanEditor.BtnSwitch.Pressed += SwitchToInspector;
  }

  private void SwitchToInspector()
  {
    ScanEditor.Visible = false;
    Inspector.Visible = true;
  }

  private void SwitchToEditor()
  {
    ScanEditor.Visible = true;
    Inspector.Visible = false;
  }
}
