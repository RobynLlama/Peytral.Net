using System;
using System.IO;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using AssemblyInspect.Sources;

if (args.Length == 0)
{
  Console.WriteLine("Usage: AssemblyInspect <assembly path>");
  Environment.ExitCode = -1;
  return;
}

string assemblyPathRaw = args[0];
string assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPathRaw);

AssemblyInspector inspector = new(assemblyPath);
var testSource = new FullCodeSource();
var testSource2 = new PInvokeRefsSource();
var extSource = new ManifestReferenceSource();
testSource.Scanners.Add(new FakeSourceScanner());
testSource2.Scanners.Add(new BasicPInvokeScanner());
extSource.Scanners.Add(new BasicReferencesScanner());
inspector.Sources.Add(testSource);
inspector.Sources.Add(testSource2);
inspector.Sources.Add(extSource);

var ctx = inspector.ScanAllSources();

foreach (var item in ctx.Observations)
  Console.WriteLine(item);
