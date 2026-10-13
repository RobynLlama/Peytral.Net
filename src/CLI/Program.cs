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

var ctx = new ScanContext(assemblyPath);
var opt = new ScanOptions();

IAssemblyScanner[] scanners = [
  new SourceScanner()
  {
    RegexRules = [
      new(@"using.*;", "Example Regex Rule", "Using statement detected", AssemblyInspect.Observations.SeverityLevel.Message)
    ],
    PlainTextRules = [
      new(@"System.", "Example Plaintext Rule", "System.* detected", AssemblyInspect.Observations.SeverityLevel.Message)
    ],
  },
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
];

var results = await AssemblyInspector.ScanAssemblyAsync(ctx, scanners, opt);

foreach (var item in results.Observations)
  Console.WriteLine(item);

Console.WriteLine("\nProject files: ");
foreach (var (name, data) in ctx.VirtualProject)
  Console.WriteLine($"  {name.TrimStart("vRoot/")}, size: {data.Length}");
