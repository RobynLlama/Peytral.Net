using System;
using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class ExamineViews : PopupPanel
{
  [Export]
  public ObservationPrefab Observation;
}
