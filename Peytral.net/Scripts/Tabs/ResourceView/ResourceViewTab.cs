using System;
using System.Linq;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using Godot;
using Peytral;

public partial class ResourceViewTab : MarginContainer
{
  [Export]
  public Label ResourcesLabel;
  [Export]
  public ResourceListBox ResourceList;

  public override void _Ready()
  {
    DocumentManager.OnDocumentClosed += ResetTab;
    DocumentManager.OnDocumentScanned += NewScan;
    ResetTab();
  }

  private void NewScan(ScanResult result)
  {
    var items = result.Observations.OfType<ResourceObservation>();
    ResourcesLabel.Text = $"Total Resources: {items.Count()}";

    foreach (var item in items)
      ResourceList.DisplayObservation(item);
  }

  private void ResetTab()
  {
    ResourceList.ClearList();
    ResourcesLabel.Text = "Total Resources: 0";
  }
}
