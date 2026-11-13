using System;
using Godot;

[GlobalClass]
public partial class ScannerElementList : VBoxContainer
{
  [Export]
  public PackedScene ElementPrefab;
  private NodePool<ScanElementPrefab> PrefabPool;
  public override void _Ready()
  {
    PrefabPool = new(PoolConstructor)
    {
      OnEnterPool = EnterPool,
    };
    PrefabPool.SetMinCapacity(20);
  }

  public void AddNewElement(bool regex = false)
  {
    var item = PrefabPool.Checkout();
    item.IsRegex = regex;
    MoveChild(item, -1);
    item.Visible = true;
  }

  public void Clear() => PrefabPool.ReturnAllToPool();

  #region Pool Functions
  private ScanElementPrefab PoolConstructor()
  {
    var child = ElementPrefab.Instantiate<ScanElementPrefab>();
    child.Visible = false;
    child.OnRemovePressed += PrefabPool.ReturnToPool;
    AddChild(child);
    return child;
  }

  private static void EnterPool(ScanElementPrefab item) =>
    item.SentToPool();
  #endregion
}
