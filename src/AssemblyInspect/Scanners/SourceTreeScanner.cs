using System;
using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;
using AssemblyInspect.Utils;

namespace AssemblyInspect.Scanners;

public class SourceTreeScanner : IAssemblyScanner
{
  public string ScannerName => "Source Tree Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public SourceTreeScanner() { }

  public SourceTreeScanner(IEnumerable<RegexRule> rules, IEnumerable<PlainTextRule> plainRules)
  {
    RegexRules = rules;
    PlainTextRules = plainRules;
  }

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    List<Observation> obs = [];

    foreach (var item in context.VirtualProject)
    {
      if (!context.ProjectLineMap.TryGetValue(item.Key, out var map))
        throw new InvalidOperationException($"Unable to fetch line map for project file: {item.Key}");

      foreach (var rule in PlainTextRules)
        foreach (var match in Utilities.FindAllOffsets(item.Value, rule.Pattern, StringComparison.OrdinalIgnoreCase))
          obs.Add(new SourceFileObservation(ScannerName, rule.Severity, rule.Summary, rule.Description, item.Key, map.GetLineIndex(match)));

      foreach (var rule in RegexRules)
        foreach (var match in rule.CompiledRegex.EnumerateMatches(item.Value))
          obs.Add(new SourceFileObservation(ScannerName, rule.Severity, rule.Summary, rule.Description, item.Key, map.GetLineIndex(match.Index)));
    }

    return obs;
  }
}
