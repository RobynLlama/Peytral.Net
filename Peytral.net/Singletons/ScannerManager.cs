using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AssemblyInspect.Scanners;
using Godot;

public static class ScannerManager
{

  public static IEnumerable<IAssemblyScanner> DefaultScanners { get; private set; } =
    ResourceLoader.Load<ScannerPackage>(@"res://Resources/Scanners/DefaultScanners.tres").BuildScanners;

  private static ScannerData Build<T>() where T : ScannerData, new() =>
      new T();

  private readonly static Dictionary<string, ScannerInfo> ScannerFactories = new()
  {
    {"sourceTreeScanner", new("Source Tree Scanner", Build<SourceTreeScannerData>)},
    {"memberRefScanner", new("Member Reference Scanner", Build<MemberReferenceScannerData>)}
  };

  public static IEnumerable<KeyValuePair<string, ScannerInfo>> EnumerateFactories() =>
    ScannerFactories.AsEnumerable();

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
