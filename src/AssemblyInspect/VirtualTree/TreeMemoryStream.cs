using System.IO;

namespace AssemblyInspect.VirtualTree;

/// <summary>
/// A fake stream that copies itself into the VTree when closed
/// </summary>
/// <param name="path">The virtual path for this stream</param>
/// <param name="tree">The VTree to send the file's bytes to</param>
/// <remarks>Flushing does nothing, the tree is not updated until the stream is closed</remarks>
public class TreeMemoryStream(string path, VTree tree) : MemoryStream
{

  private readonly string Path = path;
  private readonly VTree Tree = tree;


  public override void Close()
  {
    Tree[Path] = ToArray();
    base.Close();
  }
}
