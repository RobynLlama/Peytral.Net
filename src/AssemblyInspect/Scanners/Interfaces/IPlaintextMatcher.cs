using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Rules;

namespace AssemblyInspect.Scanners.Interfaces;

public interface IPlaintextMatcher : IAssemblyScanner
{
  public IEnumerable<PlainTextRule> PlainTextRules { get; }
}

public static partial class MatchingExtensions
{
  public static void RunPlaintextMatching(this IPlaintextMatcher self, ICollection<Observation> obs, string input, string location = "")
  {
    foreach (var item in self.PlainTextRules)
    {
      //ignore case on plaintext rules for now
      if (input.Contains(item.Pattern, System.StringComparison.OrdinalIgnoreCase))
        obs.Add(new(self.ScannerName, item.Severity, item.Summary, item.Description, location));
    }
  }
}
