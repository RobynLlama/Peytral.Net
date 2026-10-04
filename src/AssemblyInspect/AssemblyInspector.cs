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

  public ScanContext ScanAllSources()
  {
    ScanContext ctx = new(_assembly, _assemblyPath);

    foreach (var source in Sources)
      source.RunAllScanners(ctx);

    return ctx;
  }
}
