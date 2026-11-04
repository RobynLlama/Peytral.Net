using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;

namespace AssemblyInspect.Scanners;

public interface IAssemblyScanner
{
  public string ScannerName { get; }
  public IReadOnlyCollection<Observation> Scan(ScanContext context);
}
