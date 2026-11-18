using System;
using System.IO;
using Godot;

public partial class UserConfig : Resource
{
  #region Cache
  [Export]
  public bool CacheEnabled = true;
  [Export]
  public string CacheDirectory = Path.Combine(OS.GetUserDataDir(), "assembly-cache");
  #endregion

  #region Scanners
  [Export]
  public string DefaultScanners = @"res://Resources/Scanners/DefaultScanners.tres";
  #endregion

  #region Exports
  [Export]
  public bool ExportIncludesObservations = true;
  [Export]
  public bool ExportIncludesResources = true;
  [Export]
  public bool ExportIncludesFileTrees = true;
  #endregion
}
