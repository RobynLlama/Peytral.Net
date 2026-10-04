using System;
using System.IO;
using AssemblyInspect.Extensions;
using AssemblyInspect.Inspectors;

if (args.Length == 0)
{
  Console.WriteLine("Usage: AssemblyInspect <assembly path>");
  Environment.ExitCode = -1;
  return;
}

string assemblyPathRaw = args[0];
string assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPathRaw);

var inspector = new ExternInspector(assemblyPath);

foreach (var item in inspector.GetExtMemberRefs())
  Console.WriteLine(item.Describe());
foreach (var item in inspector.GetPInvokes())
  Console.WriteLine(item.DescribePInvoke());
