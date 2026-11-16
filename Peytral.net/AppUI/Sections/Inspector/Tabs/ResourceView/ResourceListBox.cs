using System.IO;
using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class ResourceListBox : VBoxContainer
{
  [Export]
  public PackedScene ResourcePrefab;
  [Export]
  public int InitialPoolSize = 50;
  [Export]
  public FileDialog ResourceFileSelector;

  private ResourceObservation CurrentResource;
  private ItemPool<ResourceViewPrefab> Pool;

  public override void _Ready()
  {
    ResourceFileSelector.FileSelected += FileSelected;
    Pool = new(CreatePoolItem);
    Pool.SetMinCapacity(InitialPoolSize);
  }

  private void FileSelected(string path) =>
    File.WriteAllBytes(path, CurrentResource.ResourceData);

  private void RequestSaveResource(ResourceObservation resource)
  {
    CurrentResource = resource;
    ResourceFileSelector.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    ResourceFileSelector.CurrentFile = resource.ResourceName;
    ResourceFileSelector.PopupCentered();
  }

  private ResourceViewPrefab CreatePoolItem()
  {
    var child = ResourcePrefab.Instantiate<ResourceViewPrefab>();
    child.Visible = false;
    child.SaveResourceCallback = RequestSaveResource;
    AddChild(child);

    return child;
  }

  public void DisplayObservation(ResourceObservation obs)
  {
    var child = Pool.Checkout();
    //Move this child to the bottom of the list because its the newest
    MoveChild(child, -1);
    child.DisplayResource(obs);
    child.Visible = true;
  }

  public void ClearList() =>
    Pool.ReturnAllToPool();
}
