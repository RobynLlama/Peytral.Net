using System;
using System.Linq;
using System.Text;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using Godot;
using Mono.Cecil;
using Peytral;

public partial class ResourceView : MarginContainer
{
  [Export]
  public Label ResourcesLabel;

  public override void _Ready()
  {
    DocumentManager.OnDocumentLoaded += NewDocument;
    DocumentManager.OnDocumentScanned += NewScan;
  }

  private void NewScan(ScanContext context)
  {
    StringBuilder sb = new("""
    Resources Found (
    """);

    Mono.Cecil.Resource[] resources = [.. context.Assembly.Modules.SelectMany(x => x.Resources)];
    sb.Append(resources.Length);
    sb.AppendLine(")\n");

    foreach (var item in resources)
    {
      sb.Append(item.Name);

      if (item is EmbeddedResource emb)
      {
        sb.Append("""
         [Embedded]
          Size: 
        """);
        sb.Append(emb.GetResourceData().Length);
        sb.AppendLine("bytes");
      }
      else if (item is AssemblyLinkedResource asm)
      {
        sb.Append("""
         [Assembly Linked]
          Target:
        """);
        sb.AppendLine(asm.Assembly.FullName);
      }
      else if (item is LinkedResource link)
      {
        sb.Append("""
         [File Linked]
          Target:
        """);
        sb.AppendLine(link.File);
      }
    }

    ResourcesLabel.Text = sb.ToString();
  }

  private void NewDocument(AssemblyInspector inspector)
  {
    ResourcesLabel.Text = string.Empty;
  }
}
