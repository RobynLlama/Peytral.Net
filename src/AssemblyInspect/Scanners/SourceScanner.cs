using System.Collections.Generic;
using AssemblyInspect.Observations;

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

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    List<Observation> obs = [];
    this.RunRegexMatches(obs, context.FullSourceBlob);
    this.RunPlaintextMatching(obs, context.FullSourceBlob);
    return obs;
  }
}
