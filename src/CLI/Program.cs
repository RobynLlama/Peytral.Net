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
var testSource = new ExternMemberRefsSource();
var testSource2 = new PInvokeRefsSource();
testSource.Scanners.Add(new MethodEchoScanner());
testSource2.Scanners.Add(new SystemExternsScanner());
//inspector.Sources.Add(testSource);
inspector.Sources.Add(testSource2);

var ctx = inspector.ScanAllSources();

foreach (var item in ctx.Observations)
  Console.WriteLine(item);
