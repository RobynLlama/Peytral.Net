using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace AssemblyInspect.Scanners;

public class FakeSourceScanner : IAssemblyScanner
{
  public void Scan(ScanContext context)
  {
    var settings = new DecompilerSettings(LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      ShowXmlDocumentation = true
    };

    var decompiler = new CSharpDecompiler(context.AssemblyLocation, settings);
    var data = decompiler.DecompileWholeModuleAsString();

    context.Observations.Add(new(nameof(FakeSourceScanner), Observations.SeverityLevel.Message, "Assembly Decompiled", $"Total size: {data.Length}", "Nowhere"));
  }
}
