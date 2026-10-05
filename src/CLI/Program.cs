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

var testSource = new FullCodeSource();
var testSource2 = new PInvokeRefsSource();
var extSource = new ManifestReferenceSource();
var memberSource = new MemberReferenceSource();
testSource.Scanners.Add(new FakeSourceScanner());
testSource2.Scanners.Add(new BasicPInvokeScanner());
extSource.Scanners.Add(new BasicReferencesScanner());
memberSource.Scanners.Add(new BasicMemberReferenceScanner());
AssemblyInspector inspector = new(assemblyPath, [
  testSource, testSource2, extSource,
  memberSource
  ]);

var ctx = inspector.ScanAllSources();

foreach (var item in ctx.Observations)
  Console.WriteLine(item);
