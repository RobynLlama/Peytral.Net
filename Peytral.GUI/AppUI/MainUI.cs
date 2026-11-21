using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Peytral.Scanners.Models;

public partial class MainUI : PanelContainer
{
  [Export]
  public InspectorSection Inspector;
  [Export]
  public ScanEditorSection ScanEditor;

  private readonly Logger.LogSink Log = new("Main GUI");
  public override void _Ready()
  {
    Inspector.BtnSwitch.Pressed += SwitchToEditor;
    ScanEditor.BtnSwitch.Pressed += SwitchToInspector;
    GetWindow().FilesDropped += HandleFileDrop;
  }

  private async void HandleFileDrop(string[] files)
  {

    using var busy = WorkingPanel.Instance.GetBusyLock();

    if (files.Length == 1)
    {
      var file = files[0];
      var ext = Path.GetExtension(file).ToLowerInvariant();
      switch (ext)
      {
        case ".dll":
          await DocumentManager.ScanSingleFile(file);
          break;
        case ".zip":
          await DocumentManager.ScanZipArchive(file);
          break;
        default:
          Log.LogMessage($"Unable to service file: {file}");
          break;
      }
    }
    else
    {
      await DocumentManager.ScanMany(files);
    }
  }

  private void SwitchToInspector()
  {
    ScanEditor.Visible = false;
    Inspector.Visible = true;
  }

  private void SwitchToEditor()
  {
    ScanEditor.Visible = true;
    Inspector.Visible = false;
  }
}
