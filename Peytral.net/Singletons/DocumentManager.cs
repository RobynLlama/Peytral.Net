using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AssemblyInspect;
using AssemblyInspect.Scanners.Models;
using Godot;

public partial class DocumentManager
{
  public static IEnumerable<ScanResult> LastScanResults { get; private set; }
  public static event Action<IEnumerable<ScanResult>> OnScanCompleted;
  public static event Action OnDocumentClosed;
  public static event Action<DirectoryInfo, IEnumerable<ScanResult>> OnDocumentSaved;

  private static readonly Logger.LogSink Log = new("Document Manager");

  public static async Task ScanSingleFile(string path)
  {

    CloseDocument();
    ScanContext ctx;

    //This will be needed later after I add proper validation
    //to context constructors
    try
    {
      ctx = new(path);
      Log.LogMessage($"Loading and scanning {Path.GetFileName(path)}");
    }
    catch (BadImageFormatException)
    {
      Log.LogWarning($"""
        Unable to scan {Path.GetFileName(path)} it doesn't appear to contain a managed assembly.
        This is expected when examining native libraries or DLLs from non-dotnet sources.
        """);
      return;
    }
    catch (Exception ex)
    {
      Log.LogError(ex.ToString());
      return;
    }

    try
    {
      LastScanResults = [await AssemblyInspector.ScanAssemblyAsync(ctx, ScannerManager.CurrentScanners.BuildScanners)];
    }
    catch (Exception ex)
    {
      Log.LogError(ex.ToString());
      return;
    }

    OnScanCompleted?.Invoke(LastScanResults);
  }

  public static async Task ScanZipArchive(string zipPath)
  {
    CloseDocument();
    FileInfo info = new(zipPath);

    if (!info.Exists)
    {
      Log.LogError($"Zip archive does not exist at path: {zipPath}");
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
          Log.LogMessage($"Extracting and scanning {item.FullName}, {deflated.Length}");
          deflated.Position = 0;

          try
          {
            contexts.Add(new(deflated, item.FullName, new($"{Path.GetFileNameWithoutExtension(item.Name)}~{prefixCount}")));
          }
          catch (BadImageFormatException)
          {
            Log.LogWarning($"""
            Skipping {item.FullName}, it doesn't appear to contain a managed assembly.
            This is expected when examining native libraries or DLLs from non-dotnet sources.
            """);
          }
          catch (Exception ex)
          {
            Log.LogError($"Encountered a {ex.GetType().Name} while loading {item.FullName}.\n{ex}");
          }
        }
      }
      LastScanResults = await AssemblyInspector.ScanAssembliesAsync(contexts, ScannerManager.CurrentScanners.BuildScanners);
    }
    catch (Exception ex)
    {
      //todo: care more about this
      Log.LogError(ex.ToString());
      return;
    }

    OnScanCompleted?.Invoke(LastScanResults);
  }

  public static async Task ScanMany(IEnumerable<string> paths)
  {
    CloseDocument();

    //validate files
    List<ScanContext> ctxs = [];

    int prefixes = 0;
    foreach (var file in paths)
      if (Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          ctxs.Add(new(file, new($"{Path.GetFileNameWithoutExtension(file)}~{++prefixes}")));
          Log.LogMessage($"Loading and scanning {Path.GetFileName(file)}");
        }
        catch (BadImageFormatException)
        {
          Log.LogWarning($"""
            Skipping {file}, it doesn't appear to contain a managed assembly.
            This is expected when examining native libraries or DLLs from non-dotnet sources.
            """);
        }
        catch (Exception ex)
        {
          Log.LogError($"Encountered a {ex.GetType().Name} while loading {file}.\n{ex}");
        }
      }
      else
        Log.LogWarning($"Ignoring non-assembly file {file}");

    LastScanResults = await AssemblyInspector.ScanAssembliesAsync(ctxs, ScannerManager.CurrentScanners.BuildScanners);
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
