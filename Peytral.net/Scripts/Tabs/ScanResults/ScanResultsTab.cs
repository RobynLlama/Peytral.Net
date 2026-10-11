using System;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

public partial class ScanResultsTab : MarginContainer
{

  [Export]
  public PackedScene ObservationPrefab;

  [Export]
  public VBoxContainer ObsContainer;

  public override void _Ready()
  {
    DocumentManager.OnDocumentLoaded += NewDocument;
    DocumentManager.OnDocumentScanned += ScannedDocument;
  }

  private void ScannedDocument(ScanContext context)
  {
    foreach (var item in context.Observations)
    {
      var child = ObservationPrefab.Instantiate<ObservationPrefab>();
      child.DisplayObservation(item);
      ObsContainer.AddChild(child);
    }
  }

  private void NewDocument(AssemblyInspector inspector)
  {
    //todo object pooling here
    foreach (var child in ObsContainer.GetChildren())
      child.QueueFree();
  }


}
