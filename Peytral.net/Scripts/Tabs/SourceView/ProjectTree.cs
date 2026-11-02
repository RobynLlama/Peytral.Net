using System;
using System.IO;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

[GlobalClass]
public partial class ProjectTree : Tree
{
  [Export]
  public Texture2D FolderIcon;
  [Export]
  public Texture2D FileIcon;
  [Export]
  public SourceCodeEditor Editor;

  public override void _Ready()
  {
    ItemActivated += SelectedItem;
    DocumentManager.OnDocumentScanned += BuildFileTreeForProject;
    DocumentManager.OnDocumentClosed += Clear;

    SetColumnExpandRatio(0, 4);
    SetColumnExpandRatio(1, 1);
  }

  private void BuildFileTreeForProject(ScanResult result)
  {
    Clear();
    var tree = result.Context.VirtualProject;
    var root = CreateItem();
    foreach (var data in tree)
    {
      var item = CreateItem(root);
      item.SetIcon(0, FileIcon);
      item.SetText(0, Path.GetFileName(data.Key));
      item.SetText(1, $"{data.Value.Length}");
      item.SetTextAlignment(1, HorizontalAlignment.Right);
      item.SetMetadata(0, data.Key);
    }
  }

  private void SelectedItem()
  {
    var selected = GetSelected();
    var path = selected.GetMetadata(0);
    GD.Print($"Selected: {path}");
    Editor.ChangeToDocument(path.ToString());
  }
}
