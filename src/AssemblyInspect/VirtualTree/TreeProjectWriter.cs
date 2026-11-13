using System.Collections.Generic;
using System.IO;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

namespace AssemblyInspect.VirtualTree;

/// <summary>
/// A WholeProjectDecompiler that redirects all CS files and the CSProj
/// into a VTree in memory rather than on the disk.
/// </summary>
/// <remarks>Does not support resource handling, use the AssemblyDefinition to read those if you want them</remarks>
public class TreeProjectWriter(IAssemblyResolver assemblyResolver, DecompilerSettings settings, PEFile file) : WholeProjectDecompiler(settings, assemblyResolver, null, null, null)
{

  //gotta do it as concurrent cuz there's some parallelism in the decomp, I think?
  public readonly VTree FileTree = [];
  public readonly PEFile Target = file;

  /// <summary>
  /// Creates a standard decompiler from default settings
  /// </summary>
  /// <param name="assemblyPath">Path to the assembly to decompile</param>
  /// <remarks>This is preferred over the constructor for ease</remarks>
  /// <returns>An instance of TreeProjectWriter</returns>
  public static TreeProjectWriter CreateFromPath(string assemblyPath)
  {
    var resolver = new UniversalAssemblyResolver(
      assemblyPath,
      throwOnError: false,
      targetFramework: null
    );

    var peFile = new PEFile(assemblyPath);
    var settings = new DecompilerSettings(ICSharpCode.Decompiler.CSharp.LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      UseNestedDirectoriesForNamespaces = true,
    };

    return new(resolver, settings, peFile);
  }


  public static TreeProjectWriter CreateFromStream(Stream assemblyData)
  {
    var resolver = new UniversalAssemblyResolver(
      string.Empty,
      throwOnError: false,
      targetFramework: null
    );

    var peFile = new PEFile("Streamed Assembly", assemblyData);
    var settings = new DecompilerSettings(ICSharpCode.Decompiler.CSharp.LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      UseNestedDirectoriesForNamespaces = true,
    };

    return new(resolver, settings, peFile);
  }

  /// <summary>
  /// Hands off the stored target to the decompiler
  /// </summary>
  /// <returns>The VTree representing the project in memory</returns>
  /// <remarks>Does not check if the VTree is clean before decompilation.
  /// It is possible to decompile multiple projects into one VTree.
  /// The user is responsible for handling the state of 
  /// their decompiler</remarks>
  public VTree DecompileProject(string rootName = "/")
  {
    DecompileProject(Target, rootName);
    return FileTree;
  }


  protected override TextWriter CreateFile(string path) =>
    FileTree.GetWriterByPath(path);

  protected override void CreateDirectory(string path)
  {
    //nop cuz we don't care about directories
    return;
  }

  protected override IEnumerable<ProjectItemInfo> WriteResourceFilesInProject(MetadataFile module)
   => [];
}
