using System;
using System.Linq;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using Godot;

namespace Peytral;

[GlobalClass]
public partial class DocumentManager : Node
{
  [Export]
  public ScannerData[] Scanners;
  public static DocumentManager Instance;
  private static AssemblyInspector OpenDocument;
  public static event Action<AssemblyInspector> OnDocumentLoaded;
  public static event Action<ScanContext> OnDocumentScanned;

  public override void _Ready()
  {
    Instance = this;
  }

  public static void OpenNewDocument(string path)
  {

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
      ctx = OpenDocument.ScanAssembly();
    }
    catch (Exception)
    {
      //todo scream here
      return;
    }

    OnDocumentScanned?.Invoke(ctx);
  }
}
