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
    var usedKey = KeyPrefix != "/";

    foreach ((string key, string item) in this)
    {
      if (usedKey)
        writer.Write(key[(KeyPrefix.Length + 1)..]);
      else
        writer.Write(key);

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
      var usedKey = keyPrefix != "/";
      for (int i = 0; i < items; i++)
      {
        string key;
        if (usedKey)
          key = Path.Combine(keyPrefix, reader.ReadString());
        else
          key = reader.ReadString();

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
