using System.Collections.Generic;

namespace AssemblyInspect.Scanners;

public interface IPlaintextMatcher : IAssemblyScanner
{
  public IEnumerable<PlainTextRule> PlainTextRules { get; }
}

public static partial class MatchingExtensions
{
  public static void RunPlaintextMatching(this IPlaintextMatcher self, ScanContext context, string input, string location = "")
  {
    foreach (var item in self.PlainTextRules)
    {
      //ignore case on plaintext rules for now
      if (input.Contains(item.Pattern, System.StringComparison.OrdinalIgnoreCase))
        context.Observations.Add(new(self.ScannerName, item.Severity, item.Summary, item.Description, location));
    }
  }
}
