using System.Collections.Generic;
using AssemblyInspect.Scanners;
using Mono.Cecil;

namespace AssemblyInspect.Sources;

public abstract class Source<T> : Source
{
  public List<IScanner<T>> Scanners = [];
  public abstract T[] GetSourceData(AssemblyDefinition assembly);
  public override void RunAllScanners(AssemblyDefinition assembly)
  {
    foreach (var scanner in Scanners)
      scanner.ScanSource(assembly, this);
  }
}
