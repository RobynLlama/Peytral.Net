using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public class ScanResult(ScanContext ctx, IEnumerable<Observation> obs)
{
  public ScanContext Context { get; } = ctx;
  public IReadOnlyList<Observation> Observations { get; } = obs.ToList().AsReadOnly();
}
