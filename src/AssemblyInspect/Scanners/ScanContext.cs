using System;
using System.Collections.Generic;
using System.IO;
using AssemblyInspect.Observations;
using AssemblyInspect.VirtualTree;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class ScanContext(AssemblyDefinition assembly, string location, IEnumerable<Observation> observations)
{
  public readonly AssemblyDefinition Assembly = assembly;
  public readonly FileInfo Info = new(location);
  public readonly IEnumerable<Observation> Observations = observations;

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

      var decompiler = new CSharpDecompiler(Info.FullName, settings);
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

      var decompiler = TreeProjectWriter.CreateFromPath(Info.FullName);
      return _virtualProject = decompiler.DecompileProject();
    }
  }

  private DocumentLineMap? _lineMap;

  public DocumentLineMap DocumentMap
  {
    get
    {
      if (_lineMap is not null)
        return _lineMap;

      return _lineMap = new(FullSourceBlob);
    }
  }
}
