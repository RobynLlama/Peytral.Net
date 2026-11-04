using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Interfaces;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;

namespace AssemblyInspect.Scanners;

public class AssemblyReferencesScanner : IRegexMatcher, IPlaintextMatcher
{

  public string ScannerName => "Assembly Reference Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    //turns out every member ref is also recorded in a
    //typedef so this is safe and faster

    var data = context.Assembly.Modules
      .SelectMany(m => m.GetTypeReferences())
      .Select(t => t.Namespace)
      .Where(ns => !string.IsNullOrEmpty(ns))
      .Distinct(StringComparer.OrdinalIgnoreCase)
      .ToHashSet();

    List<Observation> obs = [];

    foreach (var item in data)
    {
      this.RunRegexMatches(obs, item);
      this.RunPlaintextMatching(obs, item);
    }

    return obs;
  }
}
