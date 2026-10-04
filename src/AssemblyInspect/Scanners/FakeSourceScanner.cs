using AssemblyInspect.Sources;

namespace AssemblyInspect.Scanners;

public class FakeSourceScanner : IScanner<string>
{
  public void ScanSource(ScanContext context, Source<string> Source)
  {
    int size = 0;

    foreach (var item in Source.GetSourceData(context))
      size += item.Length;

    context.Observations.Add(new(nameof(FakeSourceScanner), Observations.SeverityLevel.Message, "Assembly Decompiled", $"Total size: {size}", "Nowhere"));
  }
}
