using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Resources;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;
using Godot;
using Mono.Cecil;
using Peytral;

public partial class ResourceViewTab : MarginContainer
{
  [Export]
  public Label ResourcesLabel;
  [Export]
  public VBoxContainer ResourceList;
  [Export]
  public PackedScene ResourcePrefab;
  [Export]
  public FileDialog ResourceFileSelector;

  private ResourceObservation CurrentResource;

  public override void _Ready()
  {
    DocumentManager.OnDocumentClosed += ResetTab;
    DocumentManager.OnDocumentScanned += NewScan;
    ResourceFileSelector.FileSelected += FileSelected;
    ResetTab();
  }

  private void RequestSaveResource(ResourceObservation resource)
  {
    CurrentResource = resource;
    ResourceFileSelector.CurrentDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    ResourceFileSelector.CurrentFile = resource.ResourceName;
    ResourceFileSelector.PopupCentered();
  }

  private void FileSelected(string path) =>
    File.WriteAllBytes(path, CurrentResource.ResourceData);

  private void NewScan(ScanContext context)
  {
    var items = context.Observations.OfType<ResourceObservation>();
    ResourcesLabel.Text = $"Total Resources: {items.Count()}";
    foreach (var item in items)
    {
      var child = ResourcePrefab.Instantiate<ResourceViewPrefab>();
      child.DisplayResource(item);
      child.SaveResourceCallback = RequestSaveResource;
      ResourceList.AddChild(child);
    }
  }

  private void ResetTab()
  {
    ResourcesLabel.Text = "Total Resources: 0";

    foreach (var child in ResourceList.GetChildren())
      child.QueueFree();
  }
}
