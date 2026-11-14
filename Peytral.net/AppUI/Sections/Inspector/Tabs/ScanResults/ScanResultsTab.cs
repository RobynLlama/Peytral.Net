using System.Collections;
using System.Collections.Generic;
using AssemblyInspect.Scanners.Models;
using Godot;
using Peytral;

public partial class ScanResultsTab : MarginContainer
{
  [Export]
  public ObservationList ObsContainer;

  public override void _Ready()
  {
    DocumentManager.OnDocumentClosed += ResetTab;
    DocumentManager.OnScanCompleted += ScannedDocument;
  }

  private void ScannedDocument(IEnumerable<ScanResult> results)
  {
    foreach (var scan in results)
      foreach (var item in scan.Observations)
        ObsContainer.DisplayObservation(item);
  }

  private void ResetTab()
  {
    ObsContainer.ClearList();
  }


}
