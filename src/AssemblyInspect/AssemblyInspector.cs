using System.Collections.Generic;
using AssemblyInspect.Sources;
using Mono.Cecil;

namespace AssemblyInspect;

public class AssemblyInspector
{
  protected string _assemblyPath;
  protected AssemblyDefinition _assembly;
  public List<Source> Sources = [];

  public AssemblyInspector(string path, Source[]? sources = null)
  {
    if (sources is not null)
      Sources = [.. sources];

    _assemblyPath = path;
    _assembly = AssemblyDefinition.ReadAssembly(_assemblyPath);
  }

  public void ScanAllSources()
  {
    foreach (var source in Sources)
      source.RunAllScanners(_assembly);
  }
}
