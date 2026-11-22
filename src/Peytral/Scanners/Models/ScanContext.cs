using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using Mono.Cecil;
using Peytral.Scanners.Mapping;
using Peytral.VirtualTree;

namespace Peytral.Scanners.Models;

public sealed partial class ScanContext : IDisposable
{
  public readonly AssemblyDefinition Assembly;
  private bool _disposed = false;
  private readonly MemoryStream? BackingStream;
  private readonly PEFile AssemblyModule;

  public readonly string FullName;
  public readonly long Length;

  private readonly Lazy<string> _fullSrcBlob;
  private readonly Lazy<VTree> _virtualProject;
  private readonly Lazy<DocumentLineMap> _lineMap;
  private readonly Lazy<string> _assemblyHash;
  private readonly Lazy<FullProjectLineMap> _projectLineMap;
  public string FullSourceBlob => _fullSrcBlob.Value;
  public VTree VirtualProject => _virtualProject.Value;
  public DocumentLineMap FullSourceLineMap => _lineMap.Value;
  public string AssemblyHash => _assemblyHash.Value;
  public FullProjectLineMap ProjectLineMap => _projectLineMap.Value;

  public ScanContext(string location, ScanConfiguration? config = null)
  {
    var info = new FileInfo(location);

    if (!info.Exists)
      throw new FileNotFoundException($"Unable to read {location}");

    Assembly = AssemblyDefinition.ReadAssembly(location);
    Config = config ?? new();
    FullName = location;
    AssemblyModule = new PEFile(FullName, info.OpenRead());
    Length = info.Length;
    _fullSrcBlob = new(DecompileFullProject);
    _virtualProject = new(DecompileProjectTree);
    _lineMap = new(ConstructLineMap);
    _assemblyHash = new(HashAssembly);
    _projectLineMap = new(LazyLoadProjectMap);
  }

  public ScanContext(Stream assemblyStream, string fullName = "Memory Stream", ScanConfiguration? config = null)
  {
    Length = assemblyStream.Length - assemblyStream.Position;
    BackingStream = new SharedStream();
    StreamReader reader = new(BackingStream);
    assemblyStream.CopyTo(BackingStream);
    BackingStream.Position = 0;
    Assembly = AssemblyDefinition.ReadAssembly(BackingStream);
    BackingStream.Position = 0;
    Config = config ?? new();
    AssemblyModule = new PEFile(fullName, BackingStream,
      streamOptions: System.Reflection.PortableExecutable.PEStreamOptions.PrefetchEntireImage);
    FullName = fullName;
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

    var decompiler = new CSharpDecompiler(
      AssemblyModule,
      new UniversalAssemblyResolver(string.Empty, false, null),
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
        if (VTree.TryDeserialize(Config.VTreeRootOverride, gZip, out var data))
          return data;
      }
    }

    var tree = TreeProjectWriter.CreateFromModule(AssemblyModule, Config.VTreeRootOverride).DecompileProject();

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
    string initial = $"{Assembly.FullName}|{Assembly.MainModule.Mvid}|{Length}";
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(initial)))[..24];
  }

  public void Dispose()
  {
    ObjectDisposedException.ThrowIf(_disposed, this);
    BackingStream?.Dispose();
    AssemblyModule.Dispose();
    _disposed = true;
  }

}
