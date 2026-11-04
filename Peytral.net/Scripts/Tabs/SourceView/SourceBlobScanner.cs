using System;
using System.Collections.Generic;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;

public class SourceBlobScanner : IRegexMatcher, IPlaintextMatcher
{
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public string ScannerName => "Source Blob Scanner";

  public static IEnumerable<int> FindAllOffsets(string source, string value, StringComparison comparison = StringComparison.Ordinal)
  {
    //detect whoopsie moments
    if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
      yield break;

    //step through from the first occurrence, skip if 0 occurrences
    int index = source.IndexOf(value, comparison);
    while (index != -1)
    {
      yield return index;
      //start reading from the end of the prior match
      //potential todo, maybe only advance by 1 rather than item size
      //to find substrings, not sure if that is desirable behavior
      index = source.IndexOf(value, index + value.Length, comparison);
    }
  }

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    var map = context.DocumentMap;
    var blob = context.FullSourceBlob;
    List<Observation> obs = [];

    foreach (var rule in RegexRules)
      foreach (var match in rule.CompiledRegex.EnumerateMatches(blob))
        obs.Add(new ObservationWithLineInfo(ScannerName, rule.Severity, rule.Summary, rule.Description, "Source Blob", map.GetLineIndex(match.Index)));


    //todo allow configuring the string comparer in the resource and pass
    //it in here
    foreach (var rule in PlainTextRules)
      foreach (var match in FindAllOffsets(blob, rule.Pattern, StringComparison.OrdinalIgnoreCase))
        obs.Add(new ObservationWithLineInfo(ScannerName, rule.Severity, rule.Summary, rule.Description, "Source Blob", map.GetLineIndex(match)));

    return obs;
  }
}
