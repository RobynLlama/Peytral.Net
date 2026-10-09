using Godot;

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
  public FileDialog Selector;
  public override void _Ready()
  {
    BtnNew.Pressed += NewDocument;
  }

  private void NewDocument()
  {
    Selector.PopupCentered();
  }
}
