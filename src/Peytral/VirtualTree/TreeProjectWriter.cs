using System.Collections.Generic;
using System.IO;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

namespace Peytral.VirtualTree;

/// <summary>
/// A WholeProjectDecompiler that redirects all CS files and the CSProj
/// into a VTree in memory rather than on the disk.
/// </summary>
/// <remarks>Does not support resource handling, use the AssemblyDefinition to read those if you want them</remarks>
public class TreeProjectWriter(IAssemblyResolver assemblyResolver, DecompilerSettings settings, PEFile file, string keyPrefix) : WholeProjectDecompiler(settings, assemblyResolver, null, null, null)
{
  public readonly VTree FileTree = new(keyPrefix);
  public readonly PEFile Target = file;

  /// <summary>
  /// Creates a standard decompiler from default settings
  /// </summary>
  /// <param name="assemblyPath">Path to the assembly to decompile</param>
  /// <remarks>This is preferred over the constructor for ease</remarks>
  /// <returns>An instance of TreeProjectWriter</returns>
  public static TreeProjectWriter CreateFromPath(string assemblyPath, string keyPrefix)
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

    return new(resolver, settings, peFile, keyPrefix);
  }

  public static TreeProjectWriter CreateFromModule(PEFile module, string keyPrefix)
  {

    var resolver = new UniversalAssemblyResolver(
      string.Empty,
      throwOnError: false,
      targetFramework: null
    );

    var settings = new DecompilerSettings(ICSharpCode.Decompiler.CSharp.LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      UseNestedDirectoriesForNamespaces = true,
    };

    return new(resolver, settings, module, keyPrefix);
  }

  public static TreeProjectWriter CreateFromStream(Stream assemblyData, string keyPrefix)
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

    return new(resolver, settings, peFile, keyPrefix);
  }

  /// <summary>
  /// Hands off the stored target to the decompiler
  /// </summary>
  /// <returns>The VTree representing the project in memory</returns>
  /// <remarks>Does not check if the VTree is clean before decompilation.
  /// It is possible to decompile multiple projects into one VTree.
  /// The user is responsible for handling the state of 
  /// their decompiler</remarks>
  public VTree DecompileProject()
  {
    //There is a known issue where using any rootName override
    //will make the cache unhappy, fix will be to make the root
    //override ephemeral within the cache, WIP
    DecompileProject(Target, FileTree.KeyPrefix);
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
