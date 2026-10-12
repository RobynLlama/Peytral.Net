using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;
using Mono.Cecil;

namespace AssemblyInspect;

public class AssemblyInspector
{
  protected string _assemblyPath;
  public AssemblyDefinition AssemblyDef { get; }
  public List<IAssemblyScanner> Scanners = [];

  public AssemblyInspector(string path, IEnumerable<IAssemblyScanner>? scanners = null)
  {
    if (scanners is not null)
      Scanners = [.. scanners];

    _assemblyPath = path;
    AssemblyDef = AssemblyDefinition.ReadAssembly(_assemblyPath);
  }

  public async Task<ScanContext> ScanAssemblyAsync(CancellationToken cancellationToken = default)
  {
    List<Observation> obs = [];
    ScanContext ctx = new(AssemblyDef, _assemblyPath, obs);

    //Create tasks
    Task<IReadOnlyCollection<Observation>>[] tasks = Scanners
      .Select(scanner => Task.Run(() =>
        {
          cancellationToken.ThrowIfCancellationRequested();
          var results = scanner.Scan(ctx);

          return results;
        }, cancellationToken)).ToArray();
    //Ignore the range selector hint above for readability

    //Wait
    IReadOnlyCollection<Observation>[] batchResults = await Task.WhenAll(tasks);

    foreach (var group in batchResults)
      obs.AddRange(group);

    return ctx;
  }

  public ScanContext ScanAssembly()
  {
    List<Observation> obs = [];
    ScanContext ctx = new(AssemblyDef, _assemblyPath, obs);

    foreach (var scanner in Scanners)
      obs.AddRange(scanner.Scan(ctx));

    return ctx;
  }
}
