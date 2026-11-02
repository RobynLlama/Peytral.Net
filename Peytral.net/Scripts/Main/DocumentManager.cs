using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using Godot;

namespace Peytral;

[GlobalClass]
public partial class DocumentManager : Node
{
  [Export]
  public ScannerData[] Scanners;

  public static DocumentManager Instance { get; private set; }
  public static ScanResult LastScanResult { get; private set; }
  private static readonly string CacheDirectory = Path.Combine(OS.GetUserDataDir(), "assembly-cache");
  public static event Action<ScanResult> OnDocumentScanned;
  public static event Action OnDocumentClosed;
  public static event Action<DirectoryInfo, ScanResult> OnDocumentSaved;

  public override void _Ready()
  {
    Instance = this;
    GD.Print($"Cache Dir: {CacheDirectory}");
    AssemblyInspector.SetCacheOptions(true, CacheDirectory);
  }

  public static async Task OpenNewDocument(string path)
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
      LastScanResult = await AssemblyInspector.ScanAssemblyAsync(ctx, scanners);
    }
    catch (Exception ex)
    {
      //Todo: user-facing error handling
      GD.PrintErr(ex);
      return;
    }

    OnDocumentScanned?.Invoke(LastScanResult);
  }

  public static void CloseDocument()
  {
    if (LastScanResult is null)
      return;

    LastScanResult = null;
    OnDocumentClosed?.Invoke();
  }

  public static void SaveDocument(DirectoryInfo location)
  {
    if (LastScanResult is null)
      return;

    OnDocumentSaved?.Invoke(location, LastScanResult);
  }
}
