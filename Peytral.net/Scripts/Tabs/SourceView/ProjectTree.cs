using System.Collections.Generic;
using System.Linq;
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

  public const string NO_EDIT = "NO_EDIT";

  public override void _Ready()
  {
    ItemActivated += SelectedItem;
    DocumentManager.OnDocumentScanned += BuildFileTreeForProject;
    DocumentManager.OnDocumentClosed += Clear;

    SetColumnExpandRatio(0, 4);
    SetColumnExpandRatio(1, 1);
  }

  public void BuildFileTreeForProject(ScanResult result)
  {
    //There has to be a way to make this better but this is
    //the best I've got right now
    Clear();
    TreeItem root = CreateItem();

    //cache paths to directories
    Dictionary<string, TreeItem> cache = [];
    var depthFirstVTree = result.Context.VirtualProject.OrderBy(x => x.Key, new DepthFirstComparer());

    foreach (var item in depthFirstVTree)
    {
      var fullPath = item.Key;
      string[] parts = fullPath.Split('/');

      TreeItem currentParent = root;
      string createdPath = string.Empty;

      //The last part is the file name, so don't create a directory
      //with that name, whoops
      for (int i = 0; i < parts.Length - 1; i++)
      {
        var folderName = parts[i];

        //either start with the first folder or add it to the accumulator
        createdPath = string.IsNullOrEmpty(createdPath)
          ? folderName
          : $"{createdPath}/{folderName}";

        //only add once
        if (!cache.TryGetValue(createdPath, out TreeItem dirItem))
        {
          dirItem = CreateItem(currentParent);
          dirItem.SetText(0, folderName);
          dirItem.SetIcon(0, FolderIcon);
          //this is magic to stop this item from opening in the editor
          dirItem.SetMetadata(0, NO_EDIT);
          //fold it by default
          dirItem.Collapsed = true;

          cache[createdPath] = dirItem;
        }

        //continue with this item as the new parent
        currentParent = dirItem;
      }

      var fileName = parts[^1];
      TreeItem fileItem = CreateItem(currentParent);
      fileItem.SetIcon(0, FileIcon);
      fileItem.SetText(0, fileName);
      //Add file size
      fileItem.SetText(1, item.Value.Length.ToString());
      fileItem.SetTextAlignment(1, HorizontalAlignment.Right);

      //Files that can be opened store their path in the meta
      fileItem.SetMetadata(0, fullPath);
    }
  }

  private void SelectedItem()
  {
    var selected = GetSelected();
    var path = selected.GetMetadata(0);
    GD.Print($"Selected: {path}");

    //I bet AsString() and ToString() call the same method tbh
    //but its good to be in the habit for when I expand this later
    if (path.AsString().Equals(NO_EDIT))
    {
      GD.Print("Not opening NO_EDIT item");
      return;
    }

    Editor.ChangeToDocument(path.AsString());
  }
}
