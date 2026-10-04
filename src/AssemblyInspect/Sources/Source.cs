using Mono.Cecil;

namespace AssemblyInspect.Sources;

public abstract class Source
{
  public abstract void RunAllScanners(AssemblyDefinition assembly);
}
