using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AssemblyInspect;
using AssemblyInspect.Scanners.Models;
using Godot;

namespace Peytral;

[GlobalClass]
public partial class DocumentManager : Node
{
  [Export]
  public ScannerData[] Scanners;

  public static DocumentManager Instance { get; private set; }
  public static IEnumerable<ScanResult> LastScanResults { get; private set; }
  private static readonly string CacheDirectory = Path.Combine(OS.GetUserDataDir(), "assembly-cache");
  public static event Action<IEnumerable<ScanResult>> OnScanCompleted;
  public static event Action OnDocumentClosed;
  public static event Action<DirectoryInfo, IEnumerable<ScanResult>> OnDocumentSaved;

  public override void _Ready()
  {
    Instance = this;
    GD.Print($"Cache Dir: {CacheDirectory}");
    AssemblyInspector.SetCacheOptions(true, CacheDirectory);
  }

  public static async Task ScanSingleFile(string path)
  {

    CloseDocument();
    ScanContext ctx;

    //This will be needed later after I add proper validation
    //to context constructors
    try
    {
      ctx = new(path);
    }
    catch (Exception ex)
    {
      //Todo: user-facing error handling
      GD.PrintErr(ex);
      return;
    }

    try
    {
      //Get scanners
      var scanners = Instance.Scanners
        .Select(x => x.BuildScanner());

      //Todo: allow cancellation here
      LastScanResults = [await AssemblyInspector.ScanAssemblyAsync(ctx, scanners)];
    }
    catch (Exception ex)
    {
      //Todo: user-facing error handling
      GD.PrintErr(ex);
      return;
    }

    OnScanCompleted?.Invoke(LastScanResults);
  }

  public static async Task ScanZipArchive(string zipPath)
  {
    FileInfo info = new(zipPath);

    if (!info.Exists)
    {
      GD.PrintErr($"Zip archive does not exist at path: {zipPath}");
      return;
    }

    try
    {
      var archiveStream = info.OpenRead();
      using var archive = new ZipArchive(archiveStream);
      List<ScanContext> contexts = [];
      int prefixCount = 0;

      foreach (var item in archive.Entries)
      {
        if (Path.GetExtension(item.Name).Contains(".dll", StringComparison.OrdinalIgnoreCase))
        {
          prefixCount++;
          using var itemStream = item.Open();
          var deflated = new MemoryStream();
          itemStream.CopyTo(deflated);
          GD.Print($"Extracting and scanning {item.FullName}, {deflated.Length}");
          deflated.Position = 0;

          contexts.Add(new(deflated, item.FullName, new($"{Path.GetFileNameWithoutExtension(item.Name)}~{prefixCount}")));
        }
      }

      var scanners = Instance.Scanners
        .Select(x => x.BuildScanner());

      LastScanResults = await AssemblyInspector.ScanAssembliesAsync(contexts, scanners);
    }
    catch (Exception ex)
    {
      //todo: care more about this
      GD.PrintErr(ex);
      return;
    }

    OnScanCompleted?.Invoke(LastScanResults);
  }

  public static async Task ScanMany(IEnumerable<ScanContext> contexts)
  {
    var scanners = Instance.Scanners
        .Select(x => x.BuildScanner());

    LastScanResults = await AssemblyInspector.ScanAssembliesAsync(contexts, scanners);
    OnScanCompleted?.Invoke(LastScanResults);
  }

  public static void CloseDocument()
  {
    if (LastScanResults is null)
      return;

    LastScanResults = null;
    OnDocumentClosed?.Invoke();
  }

  public static void SaveDocument(DirectoryInfo location)
  {
    if (LastScanResults is null)
      return;

    OnDocumentSaved?.Invoke(location, LastScanResults);
  }
}
