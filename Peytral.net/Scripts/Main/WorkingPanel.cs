using System;
using Godot;

[GlobalClass]
public partial class WorkingPanel : PanelContainer
{
  public static WorkingPanel Instance { get; private set; }
  private int WorkingLocks = 0;
  private static readonly object counterLock = new();

  public override void _Ready()
  {
    Instance = this;
  }

  private void HandleVisibility()
  {
    lock (counterLock)
      Visible = WorkingLocks > 0;
  }

  public BusyLock GetBusyLock()
  {
    lock (counterLock)
      WorkingLocks++;

    //Only update visibility on the main thread
    //this only matters because we do work off thread in
    //async land for the scanners
    CallDeferred(MethodName.HandleVisibility);

    return new();
  }

  private void RemoveBusyLock()
  {
    lock (counterLock)
    {
      WorkingLocks--;

      if (WorkingLocks < 0)
      {
        //unlocks the working screen
        WorkingLocks = 0;
        throw new InvalidOperationException("More locks disposed than created");
      }
    }

    //same as above
    CallDeferred(MethodName.HandleVisibility);
  }

  public sealed class BusyLock() : IDisposable
  {
    private bool _disposed = false;

    //we have to do this because it is possible to share BusyLocks
    //across work across threads. Its a BAD idea to do that and
    //surely I won't be that stupid but its a simple check
    private readonly object _disposalLock = new();
    public void Dispose()
    {
      lock (_disposalLock)
      {
        if (_disposed)
          return;

        _disposed = true;
      }
      Instance?.RemoveBusyLock();
    }
  }
}
