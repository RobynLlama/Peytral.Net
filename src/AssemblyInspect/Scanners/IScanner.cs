using AssemblyInspect.Sources;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public interface IScanner<T>
{
  public void ScanSource(ScanContext context, Source<T> Source);
}
