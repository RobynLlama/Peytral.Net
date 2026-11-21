using System.IO;

namespace Peytral.Scanners.Models;

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
