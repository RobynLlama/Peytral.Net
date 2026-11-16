using System.IO;

namespace AssemblyInspect.Scanners.Models;

public class SharedStream : MemoryStream
{

  public void DisposeInternal()
  {
    base.Dispose();
  }

  public override void Close()
  {
    //nop
  }

  public new void Dispose()
  {
    //nop
  }
}
