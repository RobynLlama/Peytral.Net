using System;
using AssemblyInspect;
using AssemblyInspect.Scanners;

namespace Peytral;

public static class DocumentManager
{
  private static AssemblyInspector OpenDocument;
  public static event Action<AssemblyInspector> OnDocumentLoaded;
  public static event Action<ScanContext> OnDocumentScanned;

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
