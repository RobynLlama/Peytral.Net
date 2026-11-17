using System;
using System.IO;
using Godot;

public partial class CurrentScannerLayout : VBoxContainer
{
  [Export]
  public Control FloatingButtonGroup;
  [Export]
  public Button AddScannerButton;
  [Export]
  public FileDialog AddScannersDialog;
  [Export]
  public PackedScene ScannerPrefab;

  private ItemPool<ScannerPrefab> Pool;

  public override void _Ready()
  {
    AddScannerButton.Pressed += ShowScannerFilePicker;
    AddScannersDialog.FilesSelected += FilesPicked;
    AddScannersDialog.FileSelected += file => FilesPicked([file]);
    Pool = new(Constructor);
    Pool.SetMinCapacity(12);
  }

  private ScannerPrefab Constructor()
  {
    var child = ScannerPrefab.Instantiate<ScannerPrefab>();
    AddChild(child);
    child.BtnDelete.Pressed += () => Pool.ReturnToPool(child);
    child.Visible = false;
    return child;
  }


  private void FilesPicked(string[] paths)
  {
    foreach (var file in paths)
    {
      if (ResourceLoader.Load<ScannerData>(file) is not ScannerData data)
      {
        GD.PrintErr($"Unable to load scanner from path: {file}");
        continue;
      }

      AddItemToScannerList(data);
    }

    //Move the floating button group to the bottom;
    MoveChild(FloatingButtonGroup, -1);
  }

  private void AddItemToScannerList(ScannerData item)
  {
    var child = Pool.Checkout();
    child.ScannerName.Text = item.ScannerNameOverride;
    child.Visible = true;
    MoveChild(child, -1);
  }

  private void ShowScannerFilePicker()
  {
    var path = Path.Combine(OS.GetUserDataDir(), "scanners", "scanner.tres");
    AddScannersDialog.CurrentPath = path;
    AddScannersDialog.PopupCentered();
  }
}
