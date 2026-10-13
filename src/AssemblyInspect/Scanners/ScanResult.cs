using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public class ScanResult(ScanContext ctx, IEnumerable<Observation> obs, ScanOptions opt)
{
  public ScanContext Context { get; } = ctx;
  public IReadOnlyList<Observation> Observations { get; } = obs.ToList().AsReadOnly();
  public ScanOptions Options { get; } = opt;
}
