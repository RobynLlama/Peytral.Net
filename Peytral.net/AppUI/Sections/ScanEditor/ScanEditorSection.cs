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
  public HelpWindow HelpDialog;
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
  [Export]
  public Button BtnHelp;


  public override void _Ready()
  {
    var dataPath = new DirectoryInfo(Path.Combine(OS.GetUserDataDir(), "scanners"));
    dataPath.Create();

    BtnNewPlain.Pressed += () => ElementList.AddNewElement(false);
    BtnNewRegex.Pressed += () => ElementList.AddNewElement(true);
    BtnClear.Pressed += ElementList.Clear;
    BtnSave.Pressed += () =>
      DialogManager.RequestSaveFile(
        SaveScannerResource,
        ["*.tres"],
        Path.Combine(dataPath.FullName, "scanner.tres"));

    var idx = 0;
    ScannerTypeDropdown.Clear();

    foreach (var item in ScannerManager.EnumerateFactories())
    {
      ScannerTypeDropdown.AddItem(item.Value.FriendlyName, idx);
      ScannerTypeDropdown.SetItemMetadata(idx, item.Key);
      idx++;
    }

    ScannerTypeDropdown.ItemSelected += ChangeScannerType;
    BtnHelp.Pressed += ShowHelpPage;
  }

  private void ShowHelpPage()
  {
    if (!ScannerManager.TryGetScannerData(ScannerTypeDropdown.GetItemMetadata(ScannerTypeDropdown.Selected).AsString(), out var data) && !data.AcceptsInput)
      return;

    HelpDialog.TextArea.Text = data.HelpText;
    HelpDialog.PopupCentered();
  }

  private void ChangeScannerType(long index)
  {
    if (ScannerManager.TryGetScannerData(ScannerTypeDropdown.GetItemMetadata((int)index).AsString(), out var data) && !data.AcceptsInput)
    {
      BtnNewPlain.Disabled = true;
      BtnNewRegex.Disabled = true;
      ElementList.Clear();
    }
    else
    {
      BtnNewPlain.Disabled = false;
      BtnNewRegex.Disabled = false;
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
