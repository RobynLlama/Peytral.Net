using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssemblyInspect.Scanners.Models;
using Godot;

[GlobalClass]
public partial class ProjectTree : Tree
{
  [Export]
  public Texture2D FolderIcon;
  [Export]
  public Texture2D FileIcon;
  [Export]
  public SourceCodeEditor Editor;

  public const string FOLD_ACTION = "FOLDER_ACTION";
  public const string NO_EDIT = "NO_EDITOR_ACTION";

  private readonly static HashSet<string> WhiteListExt = new(StringComparer.OrdinalIgnoreCase)
  {
    ".cs",
    ".csproj",
    ".txt"
  };

  public override void _Ready()
  {
    ItemActivated += SelectedItem;
    DocumentManager.OnScanCompleted += BuildFileTreeForProject;
    DocumentManager.OnDocumentClosed += Clear;

    SetColumnExpandRatio(0, 4);
    SetColumnExpandRatio(1, 1);
    SetColumnTitle(0, "Name");
    SetColumnTitle(1, "Size");
  }

  public void BuildFileTreeForProject(IEnumerable<ScanResult> results)
  {
    //There has to be a way to make this better but this is
    //the best I've got right now
    Clear();
    TreeItem root = CreateItem();

    //cache paths to directories
    Dictionary<string, TreeItem> cache = [];

    //Select all items and order by depth
    var depthFirstVTree = results
      .SelectMany(x => x.Context.VirtualProject)
      .OrderBy(x => x.Key, new DepthFirstComparer());

    foreach (var item in depthFirstVTree)
    {
      var fullPath = item.Key;
      string[] parts = fullPath.Split(Path.DirectorySeparatorChar);

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
          dirItem.SetMetadata(0, FOLD_ACTION);
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
      if (WhiteListExt.Contains(Path.GetExtension(fullPath)))
        fileItem.SetMetadata(0, fullPath);
      else
        //Only .cs files are whitelisted for the editor right now
        //This is to handle embedded resources that may show up here
        fileItem.SetMetadata(0, NO_EDIT);
    }
  }

  private void SelectedItem()
  {
    var selected = GetSelected();

    //I bet AsString() and ToString() call the same method tbh
    //but its good to be in the habit for when I expand this later
    var path = selected.GetMetadata(0).AsString();

    if (path.Equals(FOLD_ACTION))
    {
      selected.Collapsed = !selected.Collapsed;
      return;
    }
    else if (path.Equals(NO_EDIT))
    {
      return;
    }

    Editor.ChangeToDocument(path);
  }
}
