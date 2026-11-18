using System;
using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class ResourceViewPrefab : PanelContainer, IPoolItem
{

  [Export]
  public Button SaveButton;
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
  }

  private ResourceObservation CurrentResource;

  public void DisplayResource(ResourceObservation resource)
  {
    CurrentResource = resource;
    LblName.Text = $"Name: {resource.ResourceName}";
    LblLocation.Text = $"Location: {resource.ResourceLocation}";
    LblSize.Text = $"Size: {resource.ResourceData.Length:N0} bytes";
    LblAnalysis.Text = $"Analysis: [{resource.Severity}] {resource.Description}";
  }

  public void EnterPool()
  {
    Visible = false;
  }

  public void ExitPool() { }
}
