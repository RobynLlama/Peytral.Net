using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;

namespace AssemblyInspect;

public static class AssemblyInspector
{
  public static async Task<ScanResult> ScanAssemblyAsync(ScanContext ctx, IEnumerable<IAssemblyScanner> scanners, ScanOptions opt, CancellationToken cancellationToken = default)
  {
    //Create tasks
    Task<IReadOnlyCollection<Observation>>[] tasks = scanners
      .Select(scanner => Task.Run(() =>
        {
          cancellationToken.ThrowIfCancellationRequested();
          var results = scanner.Scan(ctx);

          return results;
        }, cancellationToken)).ToArray();
    //Ignore the range selector hint above for readability

    //results
    IReadOnlyList<Observation> observations = [.. (await Task.WhenAll(tasks)).SelectMany(obs => obs)];

    ScanResult results = new(ctx, observations, opt);

    return results;
  }

  public static ScanResult ScanAssembly(ScanContext ctx, IEnumerable<IAssemblyScanner> scanners, ScanOptions opt)
  {
    List<Observation> obs = [];

    foreach (var scanner in scanners)
      obs.AddRange(scanner.Scan(ctx));

    return new(ctx, obs, opt);
  }
}
