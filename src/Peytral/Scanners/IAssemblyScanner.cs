using System.Collections.Generic;
using Peytral.Observations;
using Peytral.Scanners.Models;

namespace Peytral.Scanners;

public interface IAssemblyScanner
{
  public string ScannerName { get; }
  public IReadOnlyCollection<Observation> Scan(ScanContext context);
}
