using System.Collections.Generic;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace AssemblyInspect.Scanners;

public class SourceScanner : IRegexMatcher, IPlaintextMatcher
{
  public string ScannerName => "Assembly Sourcecode Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public SourceScanner() { }

  public SourceScanner(IEnumerable<RegexRule> rules, IEnumerable<PlainTextRule> plainRules)
  {
    RegexRules = rules;
    PlainTextRules = plainRules;
  }

  public void Scan(ScanContext context)
  {
    var settings = new DecompilerSettings(LanguageVersion.Latest)
    {
      ThrowOnAssemblyResolveErrors = false,
      ShowXmlDocumentation = true
    };

    var decompiler = new CSharpDecompiler(context.AssemblyLocation, settings);
    var data = decompiler.DecompileWholeModuleAsString();

    this.RunRegexMatches(context, data);
    this.RunPlaintextMatching(context, data);
  }
}
