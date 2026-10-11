using Godot;
using Peytral;

[GlobalClass]
public partial class QuickbarPanel : MarginContainer
{
  [Export, ExportGroup("Buttons")]
  public Button BtnNew;
  [Export]
  public Button BtnSave;
  [Export]
  public Button BtnClose;
  [Export, ExportGroup("Other")]
  public FileDialog OpenFileSelector;
  public override void _Ready()
  {
    BtnNew.Pressed += () => OpenFileSelector.PopupCentered();
    BtnClose.Pressed += DocumentManager.CloseDocument;
  }
}
