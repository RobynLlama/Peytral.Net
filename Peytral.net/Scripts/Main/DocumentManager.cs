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
  private static AssemblyInspector OpenDocument;
  private static ScanContext LastScanResult;
  public static event Action<AssemblyInspector> OnDocumentLoaded;
  public static event Action<ScanContext> OnDocumentScanned;
  public static event Action OnDocumentClosed;
  public static event Action<DirectoryInfo, ScanContext> OnDocumentSaved;

  public override void _Ready()
  {
    Instance = this;
  }

  public static async Task OpenNewDocument(string path)
  {

    CloseDocument();

    try
    {
      OpenDocument = new(path);
    }
    catch (Exception)
    {
      //todo scream here
      return;
    }

    OnDocumentLoaded?.Invoke(OpenDocument);
    ScanContext ctx;

    try
    {
      OpenDocument.Scanners = [.. Instance.Scanners.Select(x => x.BuildScanner())];
      ctx = await OpenDocument.ScanAssemblyAsync();
      OnDocumentScanned?.Invoke(ctx);
      LastScanResult = ctx;
    }
    catch (Exception)
    {
      //todo scream here
      return;
    }
  }

  public static void CloseDocument()
  {
    if (OpenDocument is null)
      return;

    OpenDocument = null;
    LastScanResult = null;
    OnDocumentClosed?.Invoke();
  }

  public static void SaveDocument(DirectoryInfo location)
  {
    if (OpenDocument is null)
      return;

    if (LastScanResult is null)
      return;

    OnDocumentSaved?.Invoke(location, LastScanResult);
  }
}
