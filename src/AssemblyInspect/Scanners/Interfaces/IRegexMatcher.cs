using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Rules;

namespace AssemblyInspect.Scanners.Interfaces;

public interface IRegexMatcher : IAssemblyScanner
{
  public IEnumerable<RegexRule> RegexRules { get; }
}

public static partial class MatchingExtensions
{
  public static void RunRegexMatches(this IRegexMatcher self, ICollection<Observation> obs, string input, string location = "")
  {
    foreach (var item in self.RegexRules)
    {
      if (item.Evaluate(input))
        obs.Add(new(self.ScannerName, item.Severity, item.Summary, item.Description, location));
    }
  }
}
