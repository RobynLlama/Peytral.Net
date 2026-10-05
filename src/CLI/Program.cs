using System;
using System.IO;
using AssemblyInspect;
using AssemblyInspect.Scanners;

if (args.Length == 0)
{
  Console.WriteLine("Usage: AssemblyInspect <assembly path>");
  Environment.ExitCode = -1;
  return;
}

string assemblyPathRaw = args[0];
string assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPathRaw);

AssemblyInspector inspector = new(assemblyPath, [
  new BasicMemberReferenceScanner(),
  new BasicPInvokeScanner(),
  new BasicReferencesScanner(),
  new FakeSourceScanner(),
]);
var ctx = inspector.ScanAssembly();

foreach (var item in ctx.Observations)
  Console.WriteLine(item);
