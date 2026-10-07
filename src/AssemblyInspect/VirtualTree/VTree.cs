using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace AssemblyInspect.VirtualTree;

public class VTree : ConcurrentDictionary<string, byte[]>, IEnumerable<KeyValuePair<string, byte[]>>
{
  public StreamWriter GetWriterByPath(string path) =>
    new(new TreeMemoryStream(path, this));
}
