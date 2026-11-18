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
  public FileDialog OpenFileSelector;
  [Export]
  public FileDialog SaveReportSelector;
  [Export]
  public ConfigWindow Config;
  public override void _Ready()
  {
    BtnNew.Pressed += OpenDialogue;
    BtnSave.Pressed += OpenExportDialogue;
    BtnClose.Pressed += DocumentManager.CloseDocument;
    OpenFileSelector.FileSelected += AssemblySelected;
    SaveReportSelector.FileSelected += SaveReport;
    BtnConfig.Pressed += () => Config.PopupCentered();
  }

  private void OpenDialogue()
  {
    OpenFileSelector.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    OpenFileSelector.PopupCentered();
  }

  private void OpenExportDialogue()
  {
    SaveReportSelector.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    SaveReportSelector.PopupCentered();
  }

  private async void AssemblySelected(string path)
  {
    using var busy = WorkingPanel.Instance.GetBusyLock();
    await DocumentManager.ScanSingleFile(path);
  }

  private void SaveReport(string path)
  {
    using var working = WorkingPanel.Instance.GetBusyLock();
    using ZipArchive zip = new(new FileStream(path, FileMode.Create, System.IO.FileAccess.ReadWrite), ZipArchiveMode.Create);
    foreach (var result in DocumentManager.LastScanResults)
    {
      var prefix = result.Context.VirtualProject.KeyPrefix + '/';
      foreach (var item in result.Context.VirtualProject)
      {
        var fileName = item.Key.TrimPrefix(prefix);
        using var writer = new StreamWriter(zip.CreateEntry(Path.Combine(prefix, "project", fileName)).Open());
        writer.Write(item.Value);
      }

      foreach (var item in result.Observations.OfType<ResourceObservation>())
      {
        using var writer = zip.CreateEntry(Path.Combine(prefix, "resources", item.ResourceName)).Open();
        writer.Write(item.ResourceData);
      }

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
