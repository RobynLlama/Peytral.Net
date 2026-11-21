using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

[GlobalClass]
public partial class CurrentScannerLayout : VBoxContainer
{
  [Export]
  public Control FloatingButtonGroup;
  [Export]
  public PackedScene ScannerPrefab;

  [Export, ExportGroup("Buttons")]
  public Button AddScannerButton;
  [Export]
  public Button BtnSaveScannerPack;
  [Export]
  public Button BtnLoadScannerPack;
  [Export]
  public Button BtnApplyScannerPack;
  [Export]
  public Button BtnResetToCurrentPack;

  private ItemPool<ScannerPrefab> Pool;

  public override void _Ready()
  {
    AddScannerButton.Pressed += ShowScannerFilePicker;
    BtnLoadScannerPack.Pressed += ShowScannerPackLoadDialogue;
    BtnSaveScannerPack.Pressed += ShowScannerPackSaveDialogue;
    BtnApplyScannerPack.Pressed += ApplyScanners;
    BtnResetToCurrentPack.Pressed += ResetScannerPack;
    Pool = new(Constructor);
    Pool.SetMinCapacity(12);

    ResetScannerPack();
  }

  private void ApplyScanners()
  {
    var package = new ScannerPackage
    {
      Scanners = [.. Pool.GetActiveItems.Select(x => x.OwnedScanner)]
    };

    ScannerManager.ApplyUserScanners(package);
  }

  public void Clear() =>
    Pool.ReturnAllToPool();

  private void ResetScannerPack()
  {
    Clear();

    foreach (var scanner in ScannerManager.CurrentScanners.Scanners)
      AddItemToScannerList(scanner);
  }

  private void ShowScannerPackSaveDialogue()
  {
    var path = new DirectoryInfo(Path.Combine(OS.GetUserDataDir(), "ScannerPacks"));
    path.Create();

    DialogManager.RequestSaveFile(
      SaveScannerPackagePicked,
      ["*.tres"],
      Path.Combine(path.FullName, "ScannerPack.tres"));
  }

  private void LoadScannerPackagePicked(string path)
  {
    if (ResourceLoader.Load<ScannerPackage>(path, null, ResourceLoader.CacheMode.IgnoreDeep) is ScannerPackage pack)
    {
      Clear();
      foreach (var scanner in pack.Scanners)
        AddItemToScannerList(scanner);
    }
    else
      GD.PrintErr($"Unable to load scanners from {path}");
  }

  private void SaveScannerPackagePicked(string path)
  {
    var package = new ScannerPackage
    {
      Scanners = [.. Pool.GetActiveItems.Select(x => x.OwnedScanner)]
    };

    Error error;
    if ((error = ResourceSaver.Save(package, path)) != Error.Ok)
      GD.PrintErr($"Unable to save package to {path}\n  Error {error}");
  }

  private void ShowScannerPackLoadDialogue()
  {
    var path = new DirectoryInfo(Path.Combine(OS.GetUserDataDir(), "ScannerPacks"));
    path.Create();

    DialogManager.RequestFile(
      LoadScannerPackagePicked,
      ["*.tres"],
      Path.Combine(path.FullName, "ScannerPack.tres"));
  }

  private ScannerPrefab Constructor()
  {
    var child = ScannerPrefab.Instantiate<ScannerPrefab>();
    AddChild(child);
    child.BtnDelete.Pressed += () => Pool.ReturnToPool(child);
    child.Visible = false;
    return child;
  }

  private void ScannerFilesPicked(string[] paths)
  {
    foreach (var file in paths)
    {
      if (ResourceLoader.Load<ScannerData>(file, null, ResourceLoader.CacheMode.IgnoreDeep) is not ScannerData data)
      {
        GD.PrintErr($"Unable to load scanner from path: {file}");
        continue;
      }

      AddItemToScannerList(data);
    }
  }

  private void AddItemToScannerList(ScannerData item)
  {
    var child = Pool.Checkout();
    child.OwnedScanner = item;
    child.ScannerName.Text = item.ScannerNameOverride;
    child.Visible = true;
    MoveChild(child, -1);

    //Move the floating button group to the bottom;
    MoveChild(FloatingButtonGroup, -1);
  }

  private void ShowScannerFilePicker()
  {
    var path = Path.Combine(OS.GetUserDataDir(), "scanners", "scanner.tres");
    DialogManager.RequestFiles(
      ScannerFilesPicked,
      ["*.tres"],
      path);
  }
}
