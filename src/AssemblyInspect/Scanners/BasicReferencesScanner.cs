using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public class BasicReferencesScanner : IAssemblyScanner
{
  private static readonly Dictionary<string, (SeverityLevel Severity, string Reason)> ReferenceRules = new(StringComparer.OrdinalIgnoreCase)
  {
    ["System.Security"] = (SeverityLevel.Warning, "Assembly references cryptography/security APIs. May be used for payload decryption."),
    ["System.Security.Cryptography"] = (SeverityLevel.Warning, "Assembly references standard .NET cryptography primitives."),
    ["System.Diagnostics"] = (SeverityLevel.Warning, "Assembly references Diagnostic tools (most commonly used for Process.Start / ProcessStartInfo)"),
    ["System.Management"] = (SeverityLevel.Critical, "Assembly references Windows Management Instrumentation (WMI)."),
    ["System.Reflection.Emit"] = (SeverityLevel.Warning, "Assembly references dynamic IL generation capabilities."),
    ["System.CodeDom.Compiler"] = (SeverityLevel.Alert, "Assembly references runtime C# code compilation APIs."),
    ["Microsoft.Win32.Registry"] = (SeverityLevel.Alert, "Assembly references Windows Registry access APIs."),
    ["System.Net.Sockets"] = (SeverityLevel.Warning, "Assembly references socket networking.")
  };

  public void Scan(ScanContext context)
  {
    //we snag all the references for types and members just to cover
    //everything in every module

    var typeRefs = context.Assembly.Modules
      .SelectMany(x => x.GetTypeReferences()
        .Select(t => t.Namespace)
        .Where(ns => !string.IsNullOrEmpty(ns)));

    var memberRefNamespaces = context.Assembly.Modules
      .SelectMany(x => x.GetMemberReferences()
        .Select(m => m.DeclaringType?.Namespace)
        .OfType<string>()
        .Where(ns => ns.Length > 0));

    var data = typeRefs.Concat(memberRefNamespaces).Distinct(StringComparer.OrdinalIgnoreCase);

    foreach (var item in data)
    {
      if (ReferenceRules.TryGetValue(item, out var rule))
        context.Observations.Add(new(nameof(BasicReferencesScanner), rule.Severity, $"Reference: {item}", rule.Reason, "Assembly Metadata"));
    }
  }
}
