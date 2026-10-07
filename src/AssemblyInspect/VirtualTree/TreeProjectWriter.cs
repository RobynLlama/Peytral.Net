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
    };

    return new(resolver, settings, peFile);
  }

  /// <summary>
  /// Hands off the stored target to the decompiler
  /// at the default root "vRoot"
  /// </summary>
  /// <returns>The VTree representing the project in memory</returns>
  /// <remarks>Does not check if the VTree is clean before decompilation.
  /// It is possible to decompile multiple projects into one VTree.
  /// The user is responsible for handling the state of 
  /// their decompiler</remarks>
  public VTree DecompileProject()
  {
    DecompileProject(Target, "vRoot");
    return FileTree;
  }


  protected override TextWriter CreateFile(string path) =>
    FileTree.GetWriterByPath(path);

  protected override void CreateDirectory(string path)
  {
    //nop cuz we don't care about directories
    return;
  }

  protected override IEnumerable<ProjectItemInfo> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
  {
    //The original does this and I dunno what it does to not return the manifest so I copied it
    //Other than this, skip resources and let them be handled by querying Module.Resources elsewhere
    return [new ProjectItemInfo("EmbeddedResource", fileName).With("LogicalName", resourceName)];
  }
}
