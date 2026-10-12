using AssemblyInspect.Observations;
using Godot;

[GlobalClass]
public partial class ObservationList : VBoxContainer
{
  [Export]
  public PackedScene Prefab;
  [Export]
  public int InitialPoolSize = 50;

  private NodePool<ObservationPrefab> Pool;

  public override void _Ready()
  {
    Pool = new(InitialPoolSize, CreatePoolItem)
    {
      OnEnterPool = OnEnterPool,
    };
  }

  private ObservationPrefab CreatePoolItem()
  {
    var child = Prefab.Instantiate<ObservationPrefab>();
    child.Visible = false;
    AddChild(child);
    return child;
  }

  private static void OnEnterPool(ObservationPrefab item)
  {
    item.Visible = false;
  }

  public void DisplayObservation(Observation obs)
  {
    var child = Pool.Checkout();
    //Move this child to the bottom of the list because its the newest
    MoveChild(child, -1);
    child.DisplayObservation(obs);
    child.Visible = true;
  }

  public void ClearList() =>
    Pool.ReturnAllToPool();
}
