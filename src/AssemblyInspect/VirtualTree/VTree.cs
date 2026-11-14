using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AssemblyInspect.VirtualTree;

public class VTree(string keyPrefix) : ConcurrentDictionary<string, string>
{

  public readonly string KeyPrefix = keyPrefix;
  public StreamWriter GetWriterByPath(string path) =>
    new(new TreeMemoryStream(path, this));

  public void Serialize(Stream stream)
  {
    BinaryWriter writer = new(stream);
    //write item count
    writer.Write(Keys.Count);
    foreach ((string key, string item) in this)
    {
      writer.Write(new string(key.TrimStart(KeyPrefix)));
      writer.Write(item);
    }
  }

  public static bool TryDeserialize(string keyPrefix, Stream stream, [NotNullWhen(true)] out VTree? result)
  {
    BinaryReader reader = new(stream);
    result = new(keyPrefix);
    //read item count
    try
    {
      var items = reader.ReadInt32();
      for (int i = 0; i < items; i++)
      {
        var key = Path.Combine(keyPrefix, reader.ReadString());
        var value = reader.ReadString();
        result[key] = value;
      }

      return true;
    }
    catch (Exception)
    {
      //this is how we tell the caller we failed to deserialize
      //maybe do this better someday, dunno
      result = null;
      return false;
    }

  }
}
