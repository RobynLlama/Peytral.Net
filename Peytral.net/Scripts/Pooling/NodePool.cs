using System;
using System.Collections.Generic;
using Godot;

public class NodePool<T> where T : Node
{
  public int InitialPoolSize { get; protected set; }
  public Action<T> OnEnterPool = null;
  public Action<T> OnLeavePool = null;
  protected Func<T> Constructor;
  protected HashSet<T> ActivePoolItems = [];
  protected Stack<T> Pool = [];

  public NodePool(int size, Func<T> constructor)
  {
    Constructor = constructor;
    InitialPoolSize = size;
    InitializePool();
  }

  protected void InitializePool()
  {
    for (int i = 0; i < InitialPoolSize; i++)
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
