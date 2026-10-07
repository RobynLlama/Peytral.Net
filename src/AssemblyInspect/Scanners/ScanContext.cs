using System;
using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.VirtualTree;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class ScanContext(AssemblyDefinition assembly, string location = "")
{
  public readonly AssemblyDefinition Assembly = assembly;
  public readonly string AssemblyLocation = location;
  public readonly List<Observation> Observations = [];

  private string? _fullSrcBlob;
  public string FullSourceBlob
  {
    get
    {
      if (_fullSrcBlob is not null)
        return _fullSrcBlob;

      var settings = new DecompilerSettings(LanguageVersion.Latest)
      {
        ThrowOnAssemblyResolveErrors = false,
        ShowXmlDocumentation = true
      };

      var decompiler = new CSharpDecompiler(AssemblyLocation, settings);
      string blob = string.Empty;

      try
      {
        blob = decompiler.DecompileWholeModuleAsString();
      }
      catch (Exception)
      {
        //do something about it?
      }
      return _fullSrcBlob = blob;
    }
  }

  private VTree? _virtualProject;

  public VTree VirtualProject
  {
    get
    {
      if (_virtualProject is not null)
        return _virtualProject;

      var decompiler = TreeProjectWriter.CreateFromPath(AssemblyLocation);
      return _virtualProject = decompiler.DecompileProject();
    }
  }
}
