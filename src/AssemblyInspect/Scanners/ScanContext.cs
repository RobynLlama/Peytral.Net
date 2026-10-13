using System;
using System.IO;
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
  public string FullSourceBlob => _fullSrcBlob.Value;
  public VTree VirtualProject => _virtualProject.Value;
  public DocumentLineMap DocumentMap => _lineMap.Value;

  public ScanContext(string location)
  {
    Assembly = AssemblyDefinition.ReadAssembly(location);
    Info = new(location);
    _fullSrcBlob = new(DecompileFullProject);
    _virtualProject = new(DecompileProjectTree);
    _lineMap = new(ConstructLineMap);
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
}
