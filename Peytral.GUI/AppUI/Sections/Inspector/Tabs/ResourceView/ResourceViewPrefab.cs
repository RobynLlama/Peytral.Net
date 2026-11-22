using System;
using System.IO;
using Godot;
using Peytral.Observations;
using Peytral.Scanners.Models;

[GlobalClass]
public partial class ResourceViewPrefab : PanelContainer, IPoolItem
{

  [Export]
  public Button SaveButton;
  [Export]
  public Button ScanButton;
  [Export, ExportGroup("Text")]
  public Label LblName;
  [Export]
  public Label LblLocation;
  [Export]
  public Label LblSize;
  [Export]
  public Label LblAnalysis;

  public Action<ResourceObservation> SaveResourceCallback;

  public override void _Ready()
  {
    SaveButton.Pressed += () => SaveResourceCallback?.Invoke(CurrentResource);
    ScanButton.Pressed += ScanThisItem;
  }

  private async void ScanThisItem()
  {
    var stream = new MemoryStream(CurrentResource.ResourceData);
    var context = new ScanContext(stream, CurrentResource.ResourceName);
    await DocumentManager.ScanSingleFile(context);
  }

  private ResourceObservation CurrentResource;

  public void DisplayResource(ResourceObservation resource)
  {
    CurrentResource = resource;
    LblName.Text = $"Name: {resource.ResourceName}";
    LblLocation.Text = $"Location: {resource.ResourceLocation}";
    LblSize.Text = $"Size: {resource.ResourceData.Length:N0} bytes";
    LblAnalysis.Text = $"Analysis: [{resource.Severity}] {resource.Description}";

    ScanButton.Visible = resource.ResourceName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
  }

  public void EnterPool()
  {
    Visible = false;
  }

  public void ExitPool() { }
}
