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
  /*new SourceScanner()
  {
    RegexRules = [
      new(@"using.*;", "Example Regex Rule", "Using statement detected", AssemblyInspect.Observations.SeverityLevel.Message)
    ],
    PlainTextRules = [
      new(@"System.", "Example Plaintext Rule", "System.* detected", AssemblyInspect.Observations.SeverityLevel.Message)
    ],
  },*/
  new MemberReferenceScanner()
  {
    PlainTextRules = DemoLists.MemberRefPlainTextRules,
  },
  new PInvokeScanner()
  {
    PlainTextRules = DemoLists.PInvokePlainTextRules,
  },
  new AssemblyReferencesScanner()
  {
    PlainTextRules = DemoLists.AssemblyReferencePlainTextRules,
  },
  new ResourceScanner(),
]);

var ctx = await inspector.ScanAssemblyAsync();

foreach (var item in ctx.Observations)
  Console.WriteLine(item);

Console.WriteLine("\nProject files: ");
foreach (var (name, data) in ctx.VirtualProject)
  Console.WriteLine($"  {name.TrimStart("vRoot/")}, size: {data.Length}");
