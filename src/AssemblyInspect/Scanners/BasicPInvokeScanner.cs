using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace AssemblyInspect.Scanners;

public class BasicPInvokeScanner : IAssemblyScanner
{

  private static readonly Dictionary<string, (SeverityLevel Severity, string Reason)> Detections = new(StringComparer.OrdinalIgnoreCase)
  {
    //Critical
    ["kernel32.dll::VirtualAlloc"] = (SeverityLevel.Critical, "Dynamically allocates executable memory pages."),
    ["kernel32.dll::VirtualProtect"] = (SeverityLevel.Critical, "Modifies memory page protection flags."),
    ["kernel32.dll::WriteProcessMemory"] = (SeverityLevel.Critical, "Writes directly to process memory space."),
    ["kernel32.dll::CreateRemoteThread"] = (SeverityLevel.Critical, "Creates threads in target processes."),

    //Alert
    ["kernel32.dll::LoadLibrary"] = (SeverityLevel.Alert, "Dynamically loads native libraries at runtime to bypass static analysis."),
    ["kernel32.dll::LoadLibraryA"] = (SeverityLevel.Alert, "Dynamically loads native libraries at runtime to bypass static analysis."),
    ["kernel32.dll::LoadLibraryW"] = (SeverityLevel.Alert, "Dynamically loads native libraries at runtime to bypass static analysis."),

    //Warn
    ["kernel32.dll::GetProcAddress"] = (SeverityLevel.Alert, "Can dynamically resolve unmanaged OS features at runtime. Has a legitimate use to check if the mod is running under WINE but is dangerous in conjunction with any LoadLibrary variant."),

    //Info
    ["user32.dll::MessageBox"] = (SeverityLevel.Message, "Displays native OS dialog boxes, often used by lazy devs for debugging and not a concern on its own"),
  };

  public void Scan(ScanContext context)
  {
    bool genericWarning = false;
    var data = context.Assembly.Modules
      .SelectMany(module => module.GetAllTypes())
      .SelectMany(type => type.GetMethods())
      .Where(method => (method.Attributes & MethodAttributes.PInvokeImpl) != 0);

    foreach (var item in data)
    {
      if (!genericWarning)
      {
        genericWarning = true;
        context.Observations.Add(new(nameof(BasicPInvokeScanner), SeverityLevel.Warning, "Module loads native libraries", "The given module loads native libraries. There are some legitimate reasons for this but any native library use should be scrutinized.", "Assembly Metadata"));
      }

      string key = $"{item.PInvokeInfo.Module.Name}::{item.PInvokeInfo.EntryPoint}";

      if (Detections.TryGetValue(key, out var detection))
        context.Observations.Add(new(nameof(BasicPInvokeScanner), detection.Severity, $"Native Library ({key})", detection.Reason, "Assembly Metadata"));
      else
        context.Observations.Add(new(nameof(BasicPInvokeScanner), SeverityLevel.Message, $"Native library use ({key})", "A native library is being used, there is no specific detection information for this library but it should be examined carefully anyway.", "Assembly Metadata"));
    }
  }
}
