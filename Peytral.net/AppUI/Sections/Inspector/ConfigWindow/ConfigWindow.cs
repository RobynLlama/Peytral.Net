using System;
using System.IO;
using Godot;

[GlobalClass]
public partial class ConfigWindow : Window
{
  [Export]
  public CheckButton CacheEnabled;
  [Export]
  public LineEdit CacheLocation;
  [Export]
  public Button ClearCache;
  [Export]
  public LineEdit DefaultScanners;
  [Export]
  public Button ResetScanners;
  [Export]
  public CheckButton ExportObs;
  [Export]
  public CheckButton ExportRes;
  [Export]
  public CheckButton ExportSource;
  [Export]
  public Button ResetButton;
  [Export]
  public Button SaveButton;
  public override void _Ready()
  {
    VisibilityChanged += UpdateValues;
    SaveButton.Pressed += SaveConfig;
    ResetButton.Pressed += () => Visible = false;
    ClearCache.Pressed += ClearCacheNow;
    ResetScanners.Pressed += () => DefaultScanners.Text = @"res://Resources/Scanners/DefaultScanners.tres";
  }

  private void ClearCacheNow()
  {
    var cache = new DirectoryInfo(ConfigManager.CurrentConfig.CacheDirectory);
    if (cache.Exists)
    {
      foreach (var file in cache.GetFiles("*.gz", SearchOption.TopDirectoryOnly))
        file.Delete();
    }
  }

  private void SaveConfig()
  {
    var cfg = ConfigManager.CurrentConfig;
    cfg.CacheEnabled = CacheEnabled.ButtonPressed;
    cfg.CacheDirectory = CacheLocation.Text;
    cfg.DefaultScanners = DefaultScanners.Text;
    cfg.ExportIncludesObservations = ExportObs.ButtonPressed;
    cfg.ExportIncludesResources = ExportRes.ButtonPressed;
    cfg.ExportIncludesFileTrees = ExportSource.ButtonPressed;
    ConfigManager.SaveConfig();
    Visible = false;
  }

  private void UpdateValues()
  {
    var cfg = ConfigManager.CurrentConfig;
    CacheEnabled.ButtonPressed = cfg.CacheEnabled;
    CacheLocation.Text = cfg.CacheDirectory;
    DefaultScanners.Text = cfg.DefaultScanners;
    ExportObs.ButtonPressed = cfg.ExportIncludesObservations;
    ExportRes.ButtonPressed = cfg.ExportIncludesResources;
    ExportSource.ButtonPressed = cfg.ExportIncludesFileTrees;
  }
}
