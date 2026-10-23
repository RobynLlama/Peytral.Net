using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace AssemblyInspect.VirtualTree;

public class VTree : ConcurrentDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
{
  public StreamWriter GetWriterByPath(string path) =>
    new(new TreeMemoryStream(path, this));
}
