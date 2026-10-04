using AssemblyInspect.Extensions;
using AssemblyInspect.Sources;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class MethodEchoScanner : IScanner<MethodReference>
{
  public void ScanSource(ScanContext context, Source<MethodReference> Source)
  {
    foreach (var item in Source.GetSourceData(context))
      context.Observations.Add(new(nameof(MethodEchoScanner), Observations.SeverityLevel.Message, $"Echo ({item.Name})", item.Describe(), item.FullName));
  }
}
