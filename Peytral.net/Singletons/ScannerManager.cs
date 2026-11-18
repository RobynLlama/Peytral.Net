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
    {"sourceTreeScanner", new("Source Tree Scanner", Build<SourceTreeScannerData>, true, """
    [font_size=22][center][b]Source Tree Scanner[/b][font_size=18][left]
    Decompiles the assembly in memory and scans the raw C# source text file by file for plaintext or regex matches.

    Examples:
    [ul]Awake() (plaintext)[/ul]
    [ul](?i)(password|secret|apikey)\s*= (regex)[/ul]

    Use cases:
    [ul]Brute-force text searches across all decompiled files[/ul]
    [ul]Highlighting results in the code editor view[/ul]
    [ul]Catching strings, variable names or known source patterns[/ul]

    Drawbacks:
    [ul]No semantic awareness at all, just plain matching[/ul]
    [ul]Slower and noisier than other scanners[/ul]
    """)},
    {"pInvokeScanner", new("P/Invoke Scanner", Build<PInvokeScannerData>, true, """
    [font_size=22][center][b]P/Invoke Scanner[/b][font_size=18][left]
    Matches external native P/Invoke calls formatted as "dllname::EntryPoint" (e.g., "kernel32.dll::VirtualAlloc").

    Examples:
    [ul]kernel32.dll::VirtualAlloc (plaintext)[/ul]
    [ul](kernel32|ntdll)(\.dll)?::WriteProcessMemory (regex)[/ul]
    [ul]user32\.dll::SetWindowsHookEx (regex)[/ul]

    Use cases:
    [ul]Flagging high-risk native Win32 API calls (memory injection, hooking, unmanaged execution)[/ul]
    [ul]Building heuristic signals alongside other behavioral rules[/ul]

    Drawbacks:
    [ul]High false-positive rate; developers often use native calls for legitimate tasks even when managed alternatives exist[/ul]
    [ul]Best used as a weighted heuristic indicator rather than a standalone blocker[/ul]
    """)},
    {"memberRefScanner", new("Member Reference Scanner", Build<MemberReferenceScannerData>, true, """
    [font_size=22][center][b]Member Reference Scanner[/b][font_size=18][left]
    Matches specific types, methods, fields, and constructors using fully qualified names without assembly info (e.g., "Namespace.Type::Method").

    Examples:
    [ul]System.Diagnostics.Process::Start (plaintext)[/ul]
    [ul]System\.Reflection\.Assembly::Load (regex)[/ul]
    [ul]System\.Net\.WebClient::\.ctor (constructor regex)[/ul]

    Use cases:
    [ul]Pinpointing dangerous API calls or constructors[/ul]
    [ul]Allowing a namespace generally while blocking specific unsafe methods[/ul]

    Drawbacks:
    [ul]Only detects direct references; misses reflection or dynamic invocation[/ul]
    """)},
    {"assemblyRefScanner", new("Assembly Reference Scanner", Build<AssemblyReferenceScannerData>, true, """
    [font_size=22][center][b]Assembly Reference Scanner[/b][font_size=18][left]
    Matches top-level assembly and namespace names formatted like using directives (e.g., "System.Text.Json").

    Examples:
    [ul]System.Security.Cryptography (plaintext)[/ul]
    [ul]^Newtonsoft\.Json$ (exact regex)[/ul]

    Use cases:
    [ul]Blacklisting entire namespaces or banned libraries[/ul]
    [ul]Fast, high-level dependency audits[/ul]

    Drawbacks:
    [ul]Very broad; flags the whole reference, not specific usage[/ul]
    [ul]For targeting specific classes or methods, use MemberReferences instead.[/ul]
    """)},
    {"resourceScanner", new("Assembly Resource Scanner*", Build<ResourceScannerData>, false, """
    [font_size=22][center][b]Assembly Resource Scanner[/b][font_size=18][left]
    Inspects the leading header bytes ("magic bytes") of embedded manifest resources against built-in file signatures to identify hidden payloads.

    Configuration:
    [ul]None (no plaintext or regex patterns; detection is automatic)[/ul]

    Examples (built-in detections):
    [ul]PE executables / DLLs (MZ header)[/ul]
    [ul]ZIP archives (PK header)[/ul]
    [ul]Other common embedded binary formats[/ul]

    Use cases:
    [ul]Uncovering dropped payloads, nested assemblies, or packed archives hidden inside resources[/ul]
    [ul]Flagging suspicious binary blobs that code-level scanners ignore[/ul]

    Drawbacks:
    [ul]Not customizable; relies strictly on built-in magic byte signatures[/ul]
    [ul]Will miss encrypted, obfuscated, or resources without headers[/ul]
    """)},
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
public record struct ScannerInfo(string FriendlyName, Func<ScannerData> Constructor, bool AcceptsInput, string HelpText);
