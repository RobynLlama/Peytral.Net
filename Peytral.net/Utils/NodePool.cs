using System;
using System.Collections.Generic;
using Godot;

public class NodePool<T>(Func<T> constructor) where T : Node
{
  public Action<T> OnEnterPool = null;
  public Action<T> OnLeavePool = null;
  protected Func<T> Constructor = constructor;
  protected HashSet<T> ActivePoolItems = [];
  protected Stack<T> Pool = [];

  public IEnumerable<T> GetActiveItems => [.. ActivePoolItems];

  /// <summary>
  /// Ensures the pool has at least X number of items
  /// available for checkout.
  /// </summary>
  public void SetMinCapacity(int size)
  {
    var add = size - Pool.Count;

    if (add < 1)
      return;

    for (int i = 0; i < add; i++)
    {
      var item = Constructor();
      Pool.Push(item);
    }
  }

  public T Checkout()
  {
    if (Pool.TryPop(out var result))
    {
      ActivePoolItems.Add(result);
      OnLeavePool?.Invoke(result);
      return result;
    }

    var item = Constructor();
    ActivePoolItems.Add(item);
    OnLeavePool?.Invoke(item);
    return item;
  }

  public void ReturnToPool(T item)
  {
    ActivePoolItems.Remove(item);
    OnEnterPool?.Invoke(item);
    Pool.Push(item);
  }

  public void ReturnAllToPool()
  {
    foreach (var item in ActivePoolItems)
      ReturnToPool(item);

    ActivePoolItems.Clear();
  }
}
