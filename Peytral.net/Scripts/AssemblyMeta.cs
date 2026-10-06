using System;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

public partial class AssemblyMeta : MarginContainer
{
  [Export]
  public Label MetaLabel;
  public override void _Ready()
  {
    DocumentManager.OnDocumentScanned += ScannedDocument;
  }

  private void ScannedDocument(ScanContext context)
  {
    var culture = string.IsNullOrEmpty(context.Assembly.Name.Culture) ? "Neutral" : context.Assembly.Name.Culture;

    MetaLabel.Text = $"""
    Name: {context.Assembly.Name}
    Version: {context.Assembly.MainModule.RuntimeVersion}
    Culture: {culture}

    Location: {context.AssemblyLocation}
    """;
  }
}
