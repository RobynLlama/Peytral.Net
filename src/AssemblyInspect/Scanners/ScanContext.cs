using System.Collections.Generic;
using AssemblyInspect.Observations;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class ScanContext(AssemblyDefinition assembly, string location = "")
{
  public readonly AssemblyDefinition Assembly = assembly;
  public readonly string AssemblyLocation = location;
  public readonly List<Observation> Observations = [];

  //Todo add some basic information caching in here for scanners to reuse
}
