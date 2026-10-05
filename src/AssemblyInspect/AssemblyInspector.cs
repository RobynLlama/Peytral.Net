using System.Collections.Generic;
using AssemblyInspect.Scanners;
using Mono.Cecil;

namespace AssemblyInspect;

public class AssemblyInspector
{
  protected string _assemblyPath;
  protected AssemblyDefinition _assembly;
  public List<IAssemblyScanner> Scanners = [];

  public AssemblyInspector(string path, IEnumerable<IAssemblyScanner>? scanners = null)
  {
    if (scanners is not null)
      Scanners = [.. scanners];

    _assemblyPath = path;
    _assembly = AssemblyDefinition.ReadAssembly(_assemblyPath);
  }

  public ScanContext ScanAssembly()
  {
    ScanContext ctx = new(_assembly, _assemblyPath);

    foreach (var scanner in Scanners)
      scanner.Scan(ctx);

    return ctx;
  }
}
