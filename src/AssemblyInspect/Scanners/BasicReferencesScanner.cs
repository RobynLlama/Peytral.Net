using System;
using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Sources;

namespace AssemblyInspect.Scanners;

public class BasicReferencesScanner : IScanner<string>
{
  private static readonly Dictionary<string, (SeverityLevel Severity, string Reason)> ReferenceRules = new(StringComparer.OrdinalIgnoreCase)
  {
    ["System.Security"] = (SeverityLevel.Warning, "Assembly references cryptography/security APIs. May be used for payload decryption."),
    ["System.Security.Cryptography"] = (SeverityLevel.Warning, "Assembly references standard .NET cryptography primitives."),
    ["System.Diagnostics"] = (SeverityLevel.Warning, "Assembly references Diagnostic tools (most commonly used for Process.Start / ProcessStartInfo)"),
    ["System.Management"] = (SeverityLevel.Critical, "Assembly references Windows Management Instrumentation (WMI)."),
    ["System.Reflection.Emit"] = (SeverityLevel.Alert, "Assembly references dynamic IL generation capabilities."),
    ["System.CodeDom.Compiler"] = (SeverityLevel.Alert, "Assembly references runtime C# code compilation APIs."),
    ["Microsoft.Win32.Registry"] = (SeverityLevel.Alert, "Assembly references Windows Registry access APIs."),
    ["System.Net.Sockets"] = (SeverityLevel.Warning, "Assembly references raw TCP/UDP socket networking.")
  };

  public void ScanSource(ScanContext context, Source<string> Source)
  {
    foreach (var item in Source.GetSourceData(context))
    {
      if (ReferenceRules.TryGetValue(item, out var rule))
        context.Observations.Add(new(nameof(BasicReferencesScanner), rule.Severity, $"Reference: {item}", rule.Reason, "Assembly Metadata"));
    }
  }
}
