using System;
using System.Collections.Generic;
using System.IO;
using AssemblyInspect.Scanners.Models;
using Godot;

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
          Log.LogWarning($"Unable to service file: {file}");
          break;
      }
    }
    else
    {
      //validate files
      List<ScanContext> ctxs = [];
      int prefixes = 0;
      foreach (var file in files)
        if (Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            Log.LogMessage($"Loading and scanning {file}");
            ctxs.Add(new(file, new($"{Path.GetFileNameWithoutExtension(file)}~{++prefixes}")));
          }
          catch (Exception)
          {
            Log.LogWarning($"""
            Skipping {file}, it doesn't appear to contain a managed assembly.
            This is expected when examining native libraries or DLLs from non-dotnet sources.
            """);
          }
        }
        else
          Log.LogWarning($"Ignoring non-assembly file {file}");

      await DocumentManager.ScanMany(ctxs);
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
