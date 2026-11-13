using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public static class ScannerManager
{
  private readonly static Dictionary<string, ScannerInfo> ScannerFactories = new()
  {
    {"sourceTreeScanner", new("Source Tree Scanner", () => new SourceTreeScannerData())},
  };

  public static IEnumerator<(string ID, string FriendlyName)> EnumerateFactories()
  {
    foreach (var item in ScannerFactories)
      yield return (item.Key, item.Value.FriendlyName);
  }

  public static void AddScanner(string ID, ScannerInfo info) =>
    ScannerFactories[ID] = info;

  public static bool TryCreateScanner(string ID, [NotNullWhen(true)] out ScannerData value)
  {
    value = null;
    if (ScannerFactories.TryGetValue(ID, out var info))
    {
      value = info.Constructor();
      return true;
    }

    return false;
  }
}

public record struct ScannerInfo(string FriendlyName, Func<ScannerData> Constructor);
