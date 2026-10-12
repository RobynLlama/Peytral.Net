using AssemblyInspect.Scanners;
using Godot;
using Peytral;

public partial class ScanResultsTab : MarginContainer
{
  [Export]
  public ObservationList ObsContainer;

  public override void _Ready()
  {
    DocumentManager.OnDocumentClosed += ResetTab;
    DocumentManager.OnDocumentScanned += ScannedDocument;
  }

  private void ScannedDocument(ScanContext context)
  {
    foreach (var item in context.Observations)
      ObsContainer.DisplayObservation(item);
  }

  private void ResetTab()
  {
    ObsContainer.ClearList();
  }


}
