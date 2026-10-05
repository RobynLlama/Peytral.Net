using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace AssemblyInspect.Scanners;

public class RegexSourceScanner : RegexScannerBase, IAssemblyScanner
{

  public override string ScannerName => "Regex Source Scanner";
  public override void Scan(ScanContext context)
  {
    var settings = new DecompilerSettings(LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      ShowXmlDocumentation = true
    };

    var decompiler = new CSharpDecompiler(context.AssemblyLocation, settings);
    var data = decompiler.DecompileWholeModuleAsString();

    RunAllRules(context, data);
  }
}
