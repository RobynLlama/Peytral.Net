using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;

namespace AssemblyInspect.Scanners;

public class AssemblyReferencesScanner : SimpleScannerBase
{
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public AssemblyReferencesScanner()
  {
    ScannerName = "Assembly Reference Scanner";
  }

  public override IReadOnlyCollection<Observation> Scan(ScanContext context)
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
      RunPlainTextMatches(obs, PlainTextRules, item, "Assembly References");
      RunRegexMatches(obs, RegexRules, item, "Assembly References");
    }

    return obs;
  }
}
