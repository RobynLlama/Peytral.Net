using System;
using System.IO;
using Godot;

[GlobalClass]
public partial class ScanEditorSection : VBoxContainer
{
  [Export]
  public ScannerElementList ElementList;
  [Export]
  public OptionButton ScannerTypeDropdown;
  [Export]
  public FileDialog SaveDialog;
  [Export]
  public LineEdit ScannerName;

  [Export, ExportGroup("Buttons")]
  public Button BtnSwitch;
  [Export]
  public Button BtnNewPlain;
  [Export]
  public Button BtnNewRegex;
  [Export]
  public Button BtnSave;
  [Export]
  public Button BtnClear;


  public override void _Ready()
  {
    BtnNewPlain.Pressed += () => ElementList.AddNewElement(false);
    BtnNewRegex.Pressed += () => ElementList.AddNewElement(true);
    BtnClear.Pressed += ElementList.Clear;
    BtnSave.Pressed += () => SaveDialog.PopupCentered();

    var dataPath = new DirectoryInfo(Path.Combine(OS.GetUserDataDir(), "scanners"));
    dataPath.Create();

    SaveDialog.CurrentPath = Path.Combine(dataPath.FullName, "scanner.tres");
    SaveDialog.FileSelected += SaveScannerResource;

    var idx = 0;
    ScannerTypeDropdown.Clear();

    foreach (var item in ScannerManager.EnumerateFactories())
    {
      ScannerTypeDropdown.AddItem(item.Value.FriendlyName, idx);
      ScannerTypeDropdown.SetItemMetadata(idx, item.Key);
      idx++;
    }
  }

  private void SaveScannerResource(string path)
  {
    GD.Print($"Save resource here: {path}");

    if (!ScannerManager.TryCreateScanner(ScannerTypeDropdown.GetItemMetadata(ScannerTypeDropdown.Selected).AsString(), out var scanner))
    {
      GD.PrintErr("Invalid scanner ID");
      return;
    }

    scanner.ScannerNameOverride = ScannerName.Text;
    scanner.RegexRules = [.. ElementList.GetRegexRules()];
    scanner.PlainTextRules = [.. ElementList.GetPlainTextRules()];

    ResourceSaver.Save(scanner, path);
  }
}
