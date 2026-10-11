using System;
using System.IO;
using System.Linq;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

public partial class AssemblyMetaTab : MarginContainer
{
  [Export]
  public Label MetaLabel;
  public override void _Ready()
  {
    DocumentManager.OnDocumentScanned += ScannedDocument;
  }

  private void ScannedDocument(ScanContext context)
  {
    var culture = string.IsNullOrEmpty(context.Assembly.Name.Culture)
      ? "Neutral"
      : context.Assembly.Name.Culture;

    //I have no idea what else to put in here, it just feels so empty, lul
    var types = context.Assembly.Modules.SelectMany(x => x.GetTypes());
    var typeCount = types.Count();
    var interfaceCount = types.Count(x => x.IsInterface);
    var abstractCount = types.Count(x => x.IsAbstract);
    var classCount = types.Count(x => x.IsClass);

    MetaLabel.Text = $"""
    Name: {context.Assembly.Name.Name}
    Version: {context.Assembly.MainModule.RuntimeVersion}
    Culture: {culture}

    Location: {context.Info.FullName}
    Size: {context.Info.Length}

    Type Overview:
      Types: {typeCount}
      Classes: {classCount}
      Interfaces: {interfaceCount}
      Abstract Classes: {abstractCount}
    """;
  }
}
