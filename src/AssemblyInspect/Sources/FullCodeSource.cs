using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace AssemblyInspect.Sources;

public class FullCodeSource : Source<string>
{
  public override string[] GetSourceData(ScanContext context)
  {
    var settings = new DecompilerSettings(LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      ShowXmlDocumentation = true
    };

    var decompiler = new CSharpDecompiler(context.AssemblyLocation, settings);
    return [decompiler.DecompileWholeModuleAsString()];
  }
}
