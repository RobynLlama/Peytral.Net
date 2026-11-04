using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using AssemblyInspect.VirtualTree;

namespace AssemblyInspect.Scanners.Mapping;

public class FullProjectLineMap(VTree projectFiles) : ConcurrentDictionary<string, DocumentLineMap>
{

  private readonly VTree ProjectFiles = projectFiles;

  public new bool TryGetValue(string key, [NotNullWhen(true)] out DocumentLineMap? value)
  {
    value = null;
    //return from base if we already did this one
    if (base.TryGetValue(key, out var map))
    {
      value = map;
      return true;
    }

    //search for the file by key in our project files and make a new one
    if (ProjectFiles.TryGetValue(key, out var data))
    {
      this[key] = value = new DocumentLineMap(data);
      return true;
    }

    //Not cached, doesn't exist, shut up, die
    return false;
  }
}
