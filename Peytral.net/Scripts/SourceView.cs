using System;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

public partial class SourceView : CodeEdit
{
  public override void _Ready()
  {
    DocumentManager.OnDocumentScanned += DocumentScanned;
  }

  private void DocumentScanned(ScanContext context)
  {
    Text = context.FullSourceBlob;
  }
}
