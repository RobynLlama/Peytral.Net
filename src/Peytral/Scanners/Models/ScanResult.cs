using System.Collections.Generic;
using System.Linq;
using Peytral.Observations;

namespace Peytral.Scanners.Models;

public class ScanResult(ScanContext ctx, IEnumerable<Observation> obs)
{
  public ScanContext Context { get; } = ctx;
  public IReadOnlyList<Observation> Observations { get; } = obs.ToList().AsReadOnly();
}
