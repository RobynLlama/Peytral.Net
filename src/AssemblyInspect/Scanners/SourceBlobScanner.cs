using System;
using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;
using AssemblyInspect.Utils;

namespace AssemblyInspect.Scanners;

public class SourceBlobScanner : IAssemblyScanner
{
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public string ScannerName { get; set; } = "Source Blob Scanner";

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    List<Observation> obs = [];

    var input = context.FullSourceBlob;
    var map = context.FullSourceLineMap;

    foreach (var rule in PlainTextRules)
      foreach (var match in Utilities.FindAllOffsets(input, rule.Pattern, StringComparison.OrdinalIgnoreCase))
        obs.Add(new SourceFileObservation(ScannerName, rule.Severity, rule.Summary, rule.Description, "Source Blob", map.GetLineIndex(match)));

    foreach (var rule in RegexRules)
      foreach (var match in rule.CompiledRegex.EnumerateMatches(context.FullSourceBlob))
        obs.Add(new SourceFileObservation(ScannerName, rule.Severity, rule.Summary, rule.Description, "Source Blob", map.GetLineIndex(match.Index)));

    return obs;
  }
}
