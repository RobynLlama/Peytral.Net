using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;
using AssemblyInspect.Scanners.Models;

namespace AssemblyInspect;

public static class AssemblyInspector
{
  public static bool CacheEnabled { get; private set; } = false;
  public static string CacheDirectory { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Peytral.Net", ".cache");
  public static void SetCacheOptions(bool enabled, string location = "")
  {
    if (enabled == false)
    {
      CacheEnabled = false;
      return;
    }

    DirectoryInfo dir = new(location);
    CacheDirectory = dir.FullName;
    if (dir.Exists)
    {
      CacheEnabled = enabled;
      return;
    }

    try
    {
      dir.Create();
      CacheEnabled = enabled;
    }
    catch (Exception)
    {
      //report issue maybe?
    }
    finally
    {
      CacheEnabled = false;
    }
  }
  public static async Task<ScanResult> ScanAssemblyAsync(ScanContext ctx, IEnumerable<IAssemblyScanner> scanners, CancellationToken cancellationToken = default)
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

    ScanResult results = new(ctx, observations);

    return results;
  }

  public static ScanResult ScanAssembly(ScanContext ctx, IEnumerable<IAssemblyScanner> scanners)
  {
    List<Observation> obs = [];

    foreach (var scanner in scanners)
      obs.AddRange(scanner.Scan(ctx));

    return new(ctx, obs);
  }
}
