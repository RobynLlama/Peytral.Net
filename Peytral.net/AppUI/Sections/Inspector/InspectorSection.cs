using Godot;

[GlobalClass]
public partial class InspectorSection : VBoxContainer
{
  [Export, ExportGroup("Buttons")]
  public Button BtnNew;
  [Export]
  public Button BtnSave;
  [Export]
  public Button BtnClose;
  [Export]
  public Button BtnSwitch;
  [Export, ExportGroup("Other")]
  public FileDialog OpenFileSelector;
  public override void _Ready()
  {
    BtnNew.Pressed += OpenDialogue;
    BtnClose.Pressed += DocumentManager.CloseDocument;
    OpenFileSelector.FileSelected += AssemblySelected;
  }

  private void OpenDialogue()
  {
    OpenFileSelector.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    OpenFileSelector.PopupCentered();
  }

  private async void AssemblySelected(string path)
  {
    using var busy = WorkingPanel.Instance.GetBusyLock();
    await DocumentManager.ScanSingleFile(path);
  }
}
