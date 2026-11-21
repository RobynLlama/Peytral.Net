using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Peytral.Observations;
using Peytral.Scanners.Models;

public partial class ResourceViewTab : MarginContainer
{
  [Export]
  public Label ResourcesLabel;
  [Export]
  public ResourceListBox ResourceList;

  public override void _Ready()
  {
    DocumentManager.OnDocumentClosed += ResetTab;
    DocumentManager.OnScanCompleted += NewScan;
    ResetTab();
  }

  private void NewScan(IEnumerable<ScanResult> results)
  {

    var items = results.SelectMany(x => x.Observations.OfType<ResourceObservation>());
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
