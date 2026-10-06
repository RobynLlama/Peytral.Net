using System.Collections.Generic;

namespace AssemblyInspect.Scanners;

public interface IRegexMatcher : IAssemblyScanner
{
  public IEnumerable<RegexRule> RegexRules { get; }
}

public static partial class MatchingExtensions
{
  public static void RunRegexMatches(this IRegexMatcher self, ScanContext context, string input, string location = "")
  {
    foreach (var item in self.RegexRules)
    {
      if (item.Evaluate(input))
        context.Observations.Add(new(self.ScannerName, item.Severity, item.Summary, item.Description, location));
    }
  }
}
