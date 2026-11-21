using Godot;
using Peytral.Observations;

[GlobalClass]
public partial class ObservationList : VBoxContainer
{
  [Export]
  public PackedScene Prefab;
  [Export]
  public int InitialPoolSize = 50;

  private ItemPool<ObservationPrefab> Pool;

  public override void _Ready()
  {
    Pool = new(CreatePoolItem);
    Pool.SetMinCapacity(InitialPoolSize);
  }

  private ObservationPrefab CreatePoolItem()
  {
    var child = Prefab.Instantiate<ObservationPrefab>();
    child.Visible = false;
    AddChild(child);
    return child;
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
