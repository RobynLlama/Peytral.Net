using System;
using System.Collections.Generic;
public class ItemPool<T>(Func<T> constructor) where T : IPoolItem
{
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
    T item;
    if (Pool.TryPop(out var result))
      item = result;
    else
      item = Constructor();

    ActivePoolItems.Add(item);
    item.ExitPool();
    return item;
  }

  public void ReturnToPool(T item)
  {
    ActivePoolItems.Remove(item);
    item.EnterPool();
    Pool.Push(item);
  }

  public void ReturnAllToPool()
  {
    foreach (var item in ActivePoolItems)
      ReturnToPool(item);

    ActivePoolItems.Clear();
  }
}
