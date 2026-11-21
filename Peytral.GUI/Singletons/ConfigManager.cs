using System.IO;
using Godot;
using Peytral;

public static class ConfigManager
{

  private static readonly Logger.LogSink Log = new("Config Manager");

  static ConfigManager()
  {
    FileInfo cfg = new(ConfigLocation);
    if (cfg.Exists)
    {
      if (ResourceLoader.Load<UserConfig>(ConfigLocation, null, ResourceLoader.CacheMode.IgnoreDeep) is UserConfig config)
      {
        Log.LogMessage("Loading existing user config");
        CurrentConfig = config;
      }
    }
    else
    {
      Log.LogMessage("Creating a new user config");
      CurrentConfig = new();
      SaveConfig();
    }

    AssemblyInspector.SetCacheOptions(CurrentConfig.CacheEnabled, CurrentConfig.CacheDirectory);
  }

  private static readonly string ConfigLocation = Path.Combine(OS.GetUserDataDir(), "UserConfig.tres");
  public static UserConfig CurrentConfig { get; private set; }
  public static void SaveConfig()
  {
    ResourceSaver.Save(CurrentConfig, ConfigLocation);
    AssemblyInspector.SetCacheOptions(CurrentConfig.CacheEnabled, CurrentConfig.CacheDirectory);
  }
}
