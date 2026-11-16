using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class ResourceScanner : IAssemblyScanner
{

  public string ScannerName { get; set; } = "Resource Scanner";

  public static IEnumerable<BinaryRule> Rules { get; } =
    [
        //Binaries from all major platforms
        //Windows will probably be the only one ever useful but sure
        new BinaryRule(
            [0x4D, 0x5A],
            "PE Binary",
            "Windows PE executable or DLL payload.",
            SeverityLevel.Critical
        ),
        new BinaryRule(
            [0x7F, 0x45, 0x4C, 0x46],
            "ELF Binary",
            "Compiled Linux executable or shared library payload.",
            SeverityLevel.Critical
        ),
        new BinaryRule(
            [0xCF, 0xFA, 0xED, 0xFE],
            "Mach-O Binary",
            "Compiled macOS executable payload.",
            SeverityLevel.Critical
        ),

        //why not
        new BinaryRule(
            [0x54, 0x56, 0x71, 0x51, 0x41],
            "Base64 PE",
            "Base64-encoded Windows PE binary header",
            SeverityLevel.Critical
        ),

        //Various archives

        //Asset bundles
        new BinaryRule(
            [0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53],
            "Unity AssetBundle",
            "Embedded UnityFS AssetBundle.",
            SeverityLevel.Warning
        ),
        new BinaryRule(
            [0x55, 0x6E, 0x69, 0x74, 0x79, 0x52, 0x61, 0x72],
            "Unity AssetBundle (Legacy)",
            "Embedded legacy unity asset bundle",
            SeverityLevel.Warning
        ),
        new BinaryRule(
            [0x55, 0x6E, 0x69, 0x74, 0x79, 0x57, 0x65, 0x62],
            "Unity AssetBundle (Web)",
            "Embedded legacy Unity asset bundle for web player",
            SeverityLevel.Warning
        ),

        //Supported as ZipArchive
        new BinaryRule(
            [0x50, 0x4B, 0x03, 0x04],
            "ZIP Archive",
            "Compressed ZIP stream",
            SeverityLevel.Warning
        ),
        //Supported as GZipStream
        new BinaryRule(
            [0x1F, 0x8B],
            "GZip Stream",
            "Compressed GZip stream",
            SeverityLevel.Warning
        ),
        //Is there a binding for 7zip in C#?
        new BinaryRule(
            [0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C],
            "7-Zip Archive",
            "Compressed 7-Zip stream",
            SeverityLevel.Warning
        ),
        //I dunno maybe they distribute it and a password
        //I've seen stupider stuff than asking the user nicely
        new BinaryRule(
            [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07],
            "RAR Archive",
            "Compressed RAR stream",
            SeverityLevel.Warning
        ),
        //Support as BZipStream
        new BinaryRule(
            [0x42, 0x5A, 0x68],
            "BZip2 Stream",
            "Compressed BZip2 stream",
            SeverityLevel.Warning
        )
    ];

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    Resource[] resources = [.. context.Assembly.Modules.SelectMany(x => x.Resources)];
    List<Observation> obs = [];

    foreach (var item in resources)
    {
      //juicy pattern matching
      switch (item)
      {
        //embedded resx resource stream
        case EmbeddedResource emb when emb.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase):
          using (ResourceReader reader = new(new MemoryStream(emb.GetResourceData())))
          {
            foreach (DictionaryEntry entry in reader)
            {
              if (entry.Key?.ToString() is { } name)
              {
                reader.GetResourceData(name, out var type, out var data);
                obs.Add(CheckItem(name, data));
              }
            }
          }
          break;

        //standard embedded resource
        case EmbeddedResource emb:
          obs.Add(CheckItem(emb.Name, emb.GetResourceData()));
          break;

        //link
        case LinkedResource link:
          obs.Add(new ResourceObservation(ScannerName, SeverityLevel.Message, item.Name, "Linked File", link.File, []));
          break;

        //satellite assembly link
        case AssemblyLinkedResource asm:
          obs.Add(new ResourceObservation(ScannerName, SeverityLevel.Message, item.Name, "Assembly Link", asm.Assembly.FullName, []));
          break;
      }
    }

    return obs;
  }

  public ResourceObservation CheckItem(string name, byte[] data)
  {
    foreach (var pattern in Rules)
    {
      if (pattern.MatchesSequence(data))
        return pattern.GetObservation(ScannerName, name, data);
    }

    return new(ScannerName, SeverityLevel.Message, name, "Nothing Unusual", "No unusual signatures in this resource", data);
  }
}
