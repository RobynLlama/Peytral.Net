using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using AssemblyInspect.Scanners.Mapping;
using AssemblyInspect.VirtualTree;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using Mono.Cecil;

namespace AssemblyInspect.Scanners.Models;

public class ScanContext
{
  public readonly AssemblyDefinition Assembly;
  public readonly FileInfo Info;

  private readonly Lazy<string> _fullSrcBlob;
  private readonly Lazy<VTree> _virtualProject;
  private readonly Lazy<DocumentLineMap> _lineMap;
  private readonly Lazy<string> _assemblyHash;
  private readonly Lazy<FullProjectLineMap> _projectLineMap;
  public string FullSourceBlob => _fullSrcBlob.Value;
  public VTree VirtualProject => _virtualProject.Value;
  public DocumentLineMap FullSourceLineMap => _lineMap.Value;
  public string AssemblyHash => _assemblyHash.Value;

  public ScanContext(string location)
  {
    Assembly = AssemblyDefinition.ReadAssembly(location);
    Info = new(location);
    _fullSrcBlob = new(DecompileFullProject);
    _virtualProject = new(DecompileProjectTree);
    _lineMap = new(ConstructLineMap);
    _assemblyHash = new(HashAssembly);
    _projectLineMap = new(LazyLoadProjectMap);
  }

  private string DecompileFullProject()
  {
    if (AssemblyInspector.CacheEnabled)
    {
      //check if the item is in the cache here
      FileInfo info = new(Path.Combine(AssemblyInspector.CacheDirectory, $"{AssemblyHash}.blob.gz"));
      if (info.Exists)
      {
        GZipStream gZip = new(info.OpenRead(), CompressionMode.Decompress);
        using StreamReader reader = new(gZip);
        return reader.ReadToEnd();
      }
    }

    var decompiler = new CSharpDecompiler(Info.FullName,
      new DecompilerSettings(LanguageVersion.Latest)
      {
        ThrowOnAssemblyResolveErrors = false,
        ShowXmlDocumentation = true
      });

    string data;

    try
    {
      data = decompiler.DecompileWholeModuleAsString();
    }
    catch (Exception ex)
    {
      data = $"""
      /*
      Decompilation failed:
      {ex.Message}
      */
      """;
    }

    if (AssemblyInspector.CacheEnabled)
    {
      //cache the data
      StreamWriter blob = new(Path.Combine(AssemblyInspector.CacheDirectory, $"{AssemblyHash}.blob.gz"));
      using GZipStream gZip = new(blob.BaseStream, CompressionLevel.Optimal);
      gZip.Write(Encoding.UTF8.GetBytes(data));
    }

    return data;
  }

  private VTree DecompileProjectTree()
  {

    if (AssemblyInspector.CacheEnabled)
    {
      FileInfo info = new(Path.Combine(AssemblyInspector.CacheDirectory, $"{AssemblyHash}.tree.gz"));
      if (info.Exists)
      {
        using GZipStream gZip = new(info.OpenRead(), CompressionMode.Decompress);
        if (VTree.TryDeserialize(gZip, out var data))
          return data;
      }
    }

    var tree = TreeProjectWriter.CreateFromPath(Info.FullName).DecompileProject();

    if (AssemblyInspector.CacheEnabled)
    {
      StreamWriter treeFile = new(Path.Combine(AssemblyInspector.CacheDirectory, $"{AssemblyHash}.tree.gz"));
      using GZipStream gZip = new(treeFile.BaseStream, CompressionLevel.Optimal);
      tree.Serialize(gZip);
    }

    return tree;
  }
  private DocumentLineMap ConstructLineMap() => new(FullSourceBlob);
  private FullProjectLineMap LazyLoadProjectMap() => new(VirtualProject);
  private string HashAssembly()
  {
    string initial = $"{Assembly.FullName}|{Assembly.MainModule.Mvid}|{Info.Length}";
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(initial)))[..24];
  }
}
