using System.Collections.Generic;
using AssemblyInspect.Scanners;

namespace AssemblyInspect.Sources;

public abstract class Source<T> : Source
{
  public List<IScanner<T>> Scanners = [];
  public abstract T[] GetSourceData(ScanContext context);
  public override void RunAllScanners(ScanContext context)
  {
    foreach (var scanner in Scanners)
      scanner.ScanSource(context, this);
  }
}
