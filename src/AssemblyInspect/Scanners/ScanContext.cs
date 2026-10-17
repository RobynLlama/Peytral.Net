using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AssemblyInspect.VirtualTree;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class ScanContext
{
  public readonly AssemblyDefinition Assembly;
  public readonly FileInfo Info;

  private readonly Lazy<string> _fullSrcBlob;
  private readonly Lazy<VTree> _virtualProject;
  private readonly Lazy<DocumentLineMap> _lineMap;
  private readonly Lazy<string> _assemblyHash;
  public string FullSourceBlob => _fullSrcBlob.Value;
  public VTree VirtualProject => _virtualProject.Value;
  public DocumentLineMap DocumentMap => _lineMap.Value;
  public string AssemblyHash => _assemblyHash.Value;

  public ScanContext(string location)
  {
    Assembly = AssemblyDefinition.ReadAssembly(location);
    Info = new(location);
    _fullSrcBlob = new(DecompileFullProject);
    _virtualProject = new(DecompileProjectTree);
    _lineMap = new(ConstructLineMap);
    _assemblyHash = new(HashAssembly);
  }

  private string DecompileFullProject()
  {
    var decompiler = new CSharpDecompiler(Info.FullName,
      new DecompilerSettings(LanguageVersion.Latest)
      {
        ThrowOnAssemblyResolveErrors = false,
        ShowXmlDocumentation = true
      });

    try
    {
      return decompiler.DecompileWholeModuleAsString();
    }
    catch (Exception ex)
    {
      return $"""
      /*
      Decompilation failed:
      {ex.Message}
      */
      """;
    }
  }

  private VTree DecompileProjectTree() => TreeProjectWriter.CreateFromPath(Info.FullName).DecompileProject();
  private DocumentLineMap ConstructLineMap() => new(FullSourceBlob);
  private string HashAssembly()
  {
    string initial = $"{Assembly.FullName}|{Assembly.MainModule.Mvid}|{Info.Length}";
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(initial)))[..24];
  }
}
