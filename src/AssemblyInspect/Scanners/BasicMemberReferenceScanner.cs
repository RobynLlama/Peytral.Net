using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Extensions;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public class BasicMemberReferenceScanner : IAssemblyScanner
{

  private static readonly Dictionary<string, (SeverityLevel Severity, string Reason)> Detections = new(StringComparer.OrdinalIgnoreCase)
  {
    //Critical

    //Processes
    ["System.Diagnostics.Process::Start"] =
        (SeverityLevel.Critical, "Process management: directly spawns a new process"),
    ["System.Diagnostics.ProcessStartInfo::.ctor"] =
        (SeverityLevel.Critical, "Process management: start info is used to launch a process"),

    //Dynamic Assemblies
    ["System.Reflection.Assembly::Load"] =
        (SeverityLevel.Critical, "Assembly Loading: This overload can read from byte[], check for obfuscated or constructed assemblies"),
    ["System.Reflection.Emit.AssemblyBuilder::DefineDynamicAssembly"] =
        (SeverityLevel.Critical, "Emits dynamic IL at runtime. Used to construct assemblies in memory."),

    //Native memory junk
    ["System.Runtime.InteropServices.Marshal::GetDelegateForFunctionPointer"] =
        (SeverityLevel.Alert, "Native Marshalling: Unmanaged functions, possible externs e.g. kernel32 / user32"),
    ["System.Runtime.InteropServices.Marshal::Copy"] =
        (SeverityLevel.Alert, "Native Marshalling: Direct memory manipulation between buffers"),
    ["System.Runtime.InteropServices.Marshal::AllocHGlobal"] =
        (SeverityLevel.Alert, "Native Marshalling: unmanaged heap allocation"),


    //Alert

    //Unity Web Requests
    ["UnityEngine.Networking.UnityWebRequest::SendWebRequest"] =
        (SeverityLevel.Alert, "Network usage: Unity web request to a remote"),
    ["UnityEngine.WWW::.ctor"] =
        (SeverityLevel.Alert, "Network usage: Legacy unity web request constructor"),

    // .NET Native System HTTP Egress
    ["System.Net.Http.HttpClient::SendAsync"] =
        (SeverityLevel.Alert, "Network usage: sending messages to a remote"),
    ["System.Net.WebClient::DownloadData"] =
        (SeverityLevel.Alert, "Network usage: binary download"),
    ["System.Net.WebClient::DownloadFile"] =
        (SeverityLevel.Alert, "Network usage: file download"),
    ["System.Net.Sockets.Socket::Connect"] =
        (SeverityLevel.Alert, "Network usage via sockets"),

    //Assembly loading
    ["System.Reflection.Assembly::LoadFrom"] =
        (SeverityLevel.Alert, "Assembly loading"),
    ["System.Reflection.Assembly::LoadFile"] =
        (SeverityLevel.Alert, "Assembly loading"),


    //Warnings

    //Asset bundle nonsense
    ["UnityEngine.TextAsset::get_bytes"] =
        (SeverityLevel.Warning, "Reading binary content from a text asset can be used to hide assemblies inside asset bundles"),
    ["UnityEngine.TextAsset::get_text"] =
        (SeverityLevel.Warning, "Reading text content from a text asset can be used to hide assemblies inside asset bundles"),

    //Registry
    ["Microsoft.Win32.Registry::GetValue"] =
        (SeverityLevel.Warning, "System registry access: Reading"),
    ["Microsoft.Win32.RegistryKey::SetValue"] =
        (SeverityLevel.Warning, "System registry access: Writing"),

    //Info

    //Questionable filesystem access
    ["System.IO.Directory::Delete"] =
        (SeverityLevel.Message, "File system access: Directory removal")
  };

  public void Scan(ScanContext context)
  {
    var data = context.Assembly.Modules
      .SelectMany(module => module.GetMemberReferences());

    foreach (var item in data)
    {
      if (Detections.TryGetValue(item.ShortSignature(), out var det))
        context.Observations.Add(new(nameof(BasicMemberReferenceScanner), det.Severity, $"{item.ShortSignature()} use", det.Reason, "Assembly Member References"));
    }
  }
}
