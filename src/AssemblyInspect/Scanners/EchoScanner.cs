using System;
using AssemblyInspect.Extensions;
using AssemblyInspect.Sources;
using Mono.Cecil;

namespace AssemblyInspect.Scanners;

public class MethodEchoScanner : IScanner<MethodReference>
{
  public void ScanSource(AssemblyDefinition assembly, Source<MethodReference> Source)
  {
    Console.WriteLine("Echo scanner: ");
    foreach (var item in Source.GetSourceData(assembly))
    {
      Console.WriteLine($"  {item.Describe()}");
    }
  }
}
