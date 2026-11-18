using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;

public static class ScannerManager
{

  private static readonly ScannerPackage DefaultScanners =
    ResourceLoader.Load<ScannerPackage>(@"res://Resources/Scanners/DefaultScanners.tres");

  public static ScannerPackage CurrentScanners { get; private set; } = DefaultScanners;

  private static ScannerData Build<T>() where T : ScannerData, new() =>
      new T();

  private readonly static Dictionary<string, ScannerInfo> ScannerFactories = new()
  {
    {"sourceTreeScanner", new("Source Tree Scanner", Build<SourceTreeScannerData>, true)},
    {"pInvokeScanner", new("P/Invoke Scanner", Build<PInvokeScannerData>, true)},
    {"memberRefScanner", new("Member Reference Scanner", Build<MemberReferenceScannerData>, true)},
    {"assemblyRefScanner", new("Assembly Reference Scanner", Build<AssemblyReferenceScannerData>, true)},
    {"resourceScanner", new("Assembly Resource Scanner*", Build<ResourceScannerData>, false)},
  };

  public static IEnumerable<KeyValuePair<string, ScannerInfo>> EnumerateFactories() =>
    ScannerFactories.AsEnumerable();

  public static void AddScanner(string ID, ScannerInfo info) =>
    ScannerFactories[ID] = info;

  public static void ApplyUserScanners(ScannerPackage package) =>
    CurrentScanners = package;

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

  public static bool TryGetScannerData(string ID, out ScannerInfo value)
  {
    if (ScannerFactories.TryGetValue(ID, out var data))
    {
      value = data;
      return true;
    }

    value = new();
    return false;
  }
}
public record struct ScannerInfo(string FriendlyName, Func<ScannerData> Constructor, bool AcceptsInput);
