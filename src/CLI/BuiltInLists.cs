using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;

public static class DemoLists
{
  public static readonly PlainTextRule[] MemberRefPlainTextRules =
  [
    //Critical

    //Processes
    new("System.Diagnostics.Process::Start", "Process use", "Process management: directly spawns a new process", SeverityLevel.Critical),
    new("System.Diagnostics.ProcessStartInfo::.ctor", "Process use", "Process management: start info is used to launch a process", SeverityLevel.Critical),

    //Dynamic Assemblies
    new("System.Reflection.Assembly::Load", "Assembly Load use", "Assembly Loading: This overload can read from byte], check for obfuscated or constructed assemblies", SeverityLevel.Critical),
    new("System.Reflection.Emit.AssemblyBuilder::DefineDynamicAssembly", "Dynamic Assembly Emission", "Emits dynamic IL at runtime. Used to construct assemblies in memory.", SeverityLevel.Critical),

    //Native memory junk
    new("System.Runtime.InteropServices.Marshal::GetDelegateForFunctionPointer", "Native Delegate Use", "Native Marshalling: Unmanaged functions, possible externs e.g. kernel32 / user32", SeverityLevel.Critical),
    new("System.Runtime.InteropServices.Marshal::Copy", "Native Memory Copy Use", "Native Marshalling: Direct memory manipulation between buffers", SeverityLevel.Critical),
    new("System.Runtime.InteropServices.Marshal::AllocHGlobal", "Unmanaged Heap Allocation", "Native Marshalling: unmanaged heap allocation", SeverityLevel.Critical),

    //Alert

    //Unity Web Requests
    new("UnityEngine.Networking.UnityWebRequest::SendWebRequest", "Unity Web Request Use", "Network usage: Unity web request to a remote", SeverityLevel.Alert),
    new("UnityEngine.WWW::.ctor", "Legacy Web Request Usage", "Network usage: Legacy unity web request constructor", SeverityLevel.Alert),

    // .NET Native System HTTP Egress
    new("System.Net.Http.HttpClient::SendAsync", "HTTPClient Use", "Network usage: sending messages to a remote", SeverityLevel.Alert),
    new("System.Net.WebClient::DownloadData", "Webclient.DownloadData Use", "Network usage: binary download", SeverityLevel.Alert),
    new("System.Net.WebClient::DownloadFile", "Webclient.DownloadFile Use", "Network usage: file download", SeverityLevel.Alert),
    new("System.Net.Sockets.Socket::Connect", "Socket.Connect Use", "Network usage via sockets", SeverityLevel.Alert),

    //Assembly loading
    new("System.Reflection.Assembly::LoadFrom", "Assembly Load Use", "Assembly loading", SeverityLevel.Alert),
    new("System.Reflection.Assembly::LoadFile", "Assembly Load Use", "Assembly loading", SeverityLevel.Alert),

    //Warnings

    //Asset bundle nonsense
    new("UnityEngine.TextAsset::get_bytes", "Text Asset Use", "Reading binary content from a text asset can be used to hide assemblies inside asset bundles", SeverityLevel.Warning),
    new("UnityEngine.TextAsset::get_text", "Binary Text Asset Use", "Reading text content from a text asset can be used to hide assemblies inside asset bundles", SeverityLevel.Warning),

    //Registry
    new("Microsoft.Win32.Registry::GetValue", "Win32 Registry Read", "System registry access: Reading", SeverityLevel.Warning),
    new("Microsoft.Win32.RegistryKey::SetValue", "Win32 Registry Write", "System registry access: Writing", SeverityLevel.Warning),

    //Info

    //Questionable filesystem access
    new("System.IO.Directory::Delete", "Directory.Delete Use", "File system access: Directory removal", SeverityLevel.Message)
  ];

  public static readonly PlainTextRule[] PInvokePlainTextRules = [
    //Critical
    new("kernel32.dll::VirtualAlloc", "Virtual Alloc", "Dynamically allocates executable memory pages.", SeverityLevel.Critical),
    new("kernel32.dll::VirtualProtect", "Virtual Protect", "Modifies memory page protection flags.", SeverityLevel.Critical),
    new("kernel32.dll::WriteProcessMemory", "Write Process Memory", "Writes directly to process memory space.", SeverityLevel.Critical),
    new("kernel32.dll::CreateRemoteThread", "Create Remote Thread", "Creates threads in target processes.", SeverityLevel.Critical),

    //Alert
    new("kernel32.dll::LoadLibrary", "Load Library", "Dynamically loads native libraries at runtime to bypass static analysis.", SeverityLevel.Alert),
    new("kernel32.dll::LoadLibraryA", "Load Library", "Dynamically loads native libraries at runtime to bypass static analysis.", SeverityLevel.Alert),
    new("kernel32.dll::LoadLibraryW", "Load Library", "Dynamically loads native libraries at runtime to bypass static analysis.", SeverityLevel.Alert),

    //Warn
    new("kernel32.dll::GetProcAddress", "Get Process Address", "Can dynamically resolve unmanaged OS features at runtime. Has a legitimate use to check if the mod is running under WINE but is dangerous in conjunction with any LoadLibrary variant.", SeverityLevel.Warning),

    //Info
    new("user32.dll::MessageBox", "Message Box", "Displays native OS dialog boxes, often used by lazy devs for debugging and not a concern on its own", SeverityLevel.Message),
  ];

  public static readonly PlainTextRule[] AssemblyReferencePlainTextRules = [
    new("System.Security.Cryptography", "Cryptography Namespace", "Assembly references standard .NET cryptography.", SeverityLevel.Alert),
    new("System.Management", "Management Namespace", "Assembly references Windows Management Instrumentation (WMI).", SeverityLevel.Alert),
    new("System.Reflection.Emit", "Reflection.Emit Namespace", "Assembly references dynamic IL generation capabilities.", SeverityLevel.Warning),
    new("System.CodeDom.Compiler", "CodeDom.Compiler Namespace", "Assembly references runtime C# code compilation APIs.", SeverityLevel.Warning),
    new("Microsoft.Win32.Registry", "Win32 Registry Namespace", "Assembly references Windows Registry access APIs.", SeverityLevel.Warning),
    new("System.Net.Sockets", "Networking Sockets Namespace", "Assembly references socket networking.", SeverityLevel.Warning)
  ];
}
