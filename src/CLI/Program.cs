using System;
using System.IO;
using AssemblyInspect;
using AssemblyInspect.Scanners;
using AssemblyInspect.Scanners.Models;

if (args.Length == 0)
{
  Console.WriteLine("Usage: AssemblyInspect <assembly path>");
  Environment.ExitCode = -1;
  return;
}

string assemblyPathRaw = args[0];
string assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPathRaw);

var ctx = new ScanContext(assemblyPath);
var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Peytral.CLI", ".cache");

Console.WriteLine($"Cache directory: {cacheDir}");
AssemblyInspector.SetCacheOptions(true, cacheDir);

IAssemblyScanner[] scanners = [
  new SourceTreeScanner()
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

var results = await AssemblyInspector.ScanAssemblyAsync(ctx, scanners);

Console.WriteLine($"""
Scanned {results.Context.Assembly.Name.Name} # {ctx.AssemblyHash}
  Found {results.Observations.Count} observations

""");

foreach (var item in results.Observations)
  Console.WriteLine(item);

Console.WriteLine("\nProject files: ");
foreach (var (name, data) in ctx.VirtualProject)
  Console.WriteLine($"  {name}, size: {data.Length}");
