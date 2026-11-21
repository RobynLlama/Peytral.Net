using System.IO;
using System.IO.Compression;
using System.Linq;
using AssemblyInspect.Observations;
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
  public Button BtnConfig;
  [Export]
  public Button BtnSwitch;
  [Export, ExportGroup("Other")]
  public ConfigWindow Config;
  public override void _Ready()
  {
    BtnNew.Pressed += OpenDialogue;
    BtnSave.Pressed += OpenExportDialogue;
    BtnClose.Pressed += DocumentManager.CloseDocument;
    BtnConfig.Pressed += () => Config.PopupCentered();
  }

  private void OpenDialogue() =>
    DialogManager.RequestFile(AssemblySelected, ["*.dll, *.zip"], System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));

  private void OpenExportDialogue() =>
    DialogManager.RequestSaveFile(SaveReport, ["*.zip"], Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "results.zip"));

  private async void AssemblySelected(string path)
  {
    using var busy = WorkingPanel.Instance.GetBusyLock();
    await DocumentManager.ScanSingleFile(path);
  }

  private void SaveReport(string path)
  {
    using var working = WorkingPanel.Instance.GetBusyLock();
    using ZipArchive zip = new(new FileStream(path, FileMode.Create, System.IO.FileAccess.ReadWrite), ZipArchiveMode.Create);

    if (DocumentManager.LastScanResults is null)
    {
      GD.PrintErr("Last Scan Results is null in export, did you forget to scan something?");
      return;
    }

    foreach (var result in DocumentManager.LastScanResults)
    {
      var prefix = result.Context.VirtualProject.KeyPrefix + '/';

      if (ConfigManager.CurrentConfig.ExportIncludesFileTrees)
      {
        foreach (var item in result.Context.VirtualProject)
        {
          var fileName = item.Key.TrimPrefix(prefix);
          using var writer = new StreamWriter(zip.CreateEntry(Path.Combine(prefix, "project", fileName)).Open());
          writer.Write(item.Value);
        }
      }

      if (ConfigManager.CurrentConfig.ExportIncludesResources)
      {
        foreach (var item in result.Observations.OfType<ResourceObservation>())
        {
          using var writer = zip.CreateEntry(Path.Combine(prefix, "resources", item.ResourceName)).Open();
          writer.Write(item.ResourceData);
        }
      }

      if (ConfigManager.CurrentConfig.ExportIncludesObservations)
      {
        using var obsWriter = new StreamWriter(zip.CreateEntry(Path.Combine(prefix, "observations.txt")).Open());
        foreach (var item in result.Observations)
        {
          obsWriter.Write($"""
        {item.GetType().Name} ({item.Severity}) — {item.Summary}
        Detected By: {item.ScannerName}
        Message: {item.Description}


        """);
        }
      }

    }
  }
}
