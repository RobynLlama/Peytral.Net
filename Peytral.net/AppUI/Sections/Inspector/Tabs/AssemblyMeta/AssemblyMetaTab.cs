using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyInspect.Scanners.Models;
using Godot;
using Peytral;

public partial class AssemblyMetaTab : MarginContainer
{
  [Export]
  public Label MetaLabel;
  [Export]
  public string PlaceholderText;
  public override void _Ready()
  {
    DocumentManager.OnScanCompleted += ScannedDocument;
    DocumentManager.OnDocumentClosed += ResetTab;
    ResetTab();
  }

  private void ResetTab()
  {
    MetaLabel.Text = PlaceholderText;
  }

  private void ScannedDocument(IEnumerable<ScanResult> results)
  {
    StringBuilder sb = new("Scanned Assemblies:\n\n");
    foreach (var result in results)
    {
      //lazy update
      var context = result.Context;

      var culture = string.IsNullOrEmpty(context.Assembly.Name.Culture)
        ? "Neutral"
        : context.Assembly.Name.Culture;

      //I have no idea what else to put in here, it just feels so empty, lul
      var types = context.Assembly.Modules.SelectMany(x => x.GetTypes());
      var typeCount = types.Count();
      var interfaceCount = types.Count(x => x.IsInterface);
      var abstractCount = types.Count(x => x.IsAbstract);
      var classCount = types.Count(x => x.IsClass);

      sb.Append($"""
        Name: {context.Assembly.Name.Name}
        Version: {context.Assembly.MainModule.RuntimeVersion}
        Culture: {culture}

        Location: {context.FullName}
        Size: {context.Length}

        Type Overview:
          Types: {typeCount}
          Classes: {classCount}
          Interfaces: {interfaceCount}
          Abstract Classes: {abstractCount}

        """);
    }

    MetaLabel.Text = sb.ToString();
  }
}
