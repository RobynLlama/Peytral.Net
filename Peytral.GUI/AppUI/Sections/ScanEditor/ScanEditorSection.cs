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
  [Export]
  public LineEdit ScannerDesc;

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
  public Button BtnLoad;
  [Export]
  public Button BtnHelp;

  private readonly Logger.LogSink Log = new("Scan Editor");

  public override void _Ready()
  {
    var dataPath = new DirectoryInfo(Path.Combine(OS.GetUserDataDir(), "scanners"));
    dataPath.Create();

    BtnNewPlain.Pressed += () => ElementList.AddNewElement(false);
    BtnNewRegex.Pressed += () => ElementList.AddNewElement(true);
    BtnLoad.Pressed += () => DialogManager.RequestFile(LoadScanner, ["*.tres"], Path.Combine(OS.GetUserDataDir(), "scanners", "scanner.tres"));
    BtnClear.Pressed += () =>
    {
      ElementList.Clear();
      ScannerName.Text = string.Empty;
      ScannerDesc.Text = string.Empty;
    };
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

  private void LoadScanner(string path)
  {
    FileInfo info = new(path);
    if (!info.Exists)
    {
      Log.LogWarning($"Unable to load scanner from path: {path}, file does not exist");
      return;
    }

    if (ResourceLoader.Load<ScannerData>(path, null, ResourceLoader.CacheMode.IgnoreDeep) is not ScannerData data)
    {
      Log.LogError($"Resource at location {path} is not a valid scanner data or is corrupted");
      return;
    }

    ElementList.Clear();
    ScannerName.Text = data.ScannerNameOverride;
    ScannerDesc.Text = data.ScannerEditorDescription;
    var scannerTypeName = data.GetType().Name;

    var maxItems = ScannerTypeDropdown.ItemCount;
    var finalItem = maxItems - 1;

    for (int i = 0; i < maxItems; i++)
    {
      var item = ScannerTypeDropdown.GetItemMetadata(i).AsString();

      if (!scannerTypeName.Equals(item))
      {
        //final item without finding the match
        if (i == finalItem)
        {
          Log.LogError($"Unable to find a valid type for {scannerTypeName}, aborting scanner load.");
          return;
        }
        continue;
      }

      ScannerTypeDropdown.Selected = i;
      break;
    }

    foreach (var item in data.PlainTextRules)
      ElementList.AddNewElement(false, item.Summary, item.Pattern, item.Description, item.Severity);
    foreach (var item in data.RegexRules)
      ElementList.AddNewElement(true, item.Summary, item.Pattern, item.Description, item.Severity);
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
    if (!ScannerManager.TryCreateScanner(ScannerTypeDropdown.GetItemMetadata(ScannerTypeDropdown.Selected).AsString(), out var scanner))
    {
      Log.LogError("Invalid scanner ID!");
      return;
    }

    scanner.ScannerNameOverride = ScannerName.Text;
    scanner.ScannerEditorDescription = ScannerDesc.Text;
    scanner.RegexRules = [.. ElementList.GetRegexRules()];
    scanner.PlainTextRules = [.. ElementList.GetPlainTextRules()];

    ResourceSaver.Save(scanner, path);
  }
}
