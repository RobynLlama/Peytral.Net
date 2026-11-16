using System;
using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;
using AssemblyInspect.Utils;

namespace AssemblyInspect.Scanners;

/// <summary>
/// A simple base scanner for scanners that do not have a location
/// to report. EG assembly meta scanners, reference scanners, etc
/// </summary>
public abstract class SimpleScannerBase : IAssemblyScanner
{
  public string ScannerName { get; set; } = "Name not set";
  public abstract IReadOnlyCollection<Observation> Scan(ScanContext context);

  protected void RunPlainTextMatches(ICollection<Observation> obs, IEnumerable<PlainTextRule> rules, string input, string Location)
  {
    foreach (var rule in rules)
      foreach (var _ in Utilities.FindAllOffsets(input, rule.Pattern, StringComparison.OrdinalIgnoreCase))
        obs.Add(new Observation(ScannerName, rule.Severity, rule.Summary, rule.Description, Location));
  }

  protected void RunRegexMatches(ICollection<Observation> obs, IEnumerable<RegexRule> rules, string input, string Location)
  {
    foreach (var rule in rules)
      foreach (var _ in rule.CompiledRegex.EnumerateMatches(input))
        obs.Add(new Observation(ScannerName, rule.Severity, rule.Summary, rule.Description, Location));
  }
}
