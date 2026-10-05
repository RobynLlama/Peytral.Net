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

var regexScan = new RegexSourceScanner();
regexScan.RegexRules.Add(
  new(@"using.*;", "Example Regex Rule", "Using statement detected", AssemblyInspect.Observations.SeverityLevel.Message)
);

AssemblyInspector inspector = new(assemblyPath, [
  new BasicMemberReferenceScanner(),
  new BasicPInvokeScanner(),
  new BasicReferencesScanner(),
  regexScan,
]);
var ctx = inspector.ScanAssembly();

foreach (var item in ctx.Observations)
  Console.WriteLine(item);
