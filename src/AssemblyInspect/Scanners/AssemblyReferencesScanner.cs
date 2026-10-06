using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public class AssemblyReferencesScanner : IRegexMatcher, IPlaintextMatcher
{

  public string ScannerName => "Assembly Reference Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public void Scan(ScanContext context)
  {
    //we snag all the references for types and members just to cover
    //everything in every module

    var typeRefs = context.Assembly.Modules
      .SelectMany(x => x.GetTypeReferences()
        .Select(t => t.Namespace)
        .Where(ns => !string.IsNullOrEmpty(ns)));

    var memberRefNamespaces = context.Assembly.Modules
      .SelectMany(x => x.GetMemberReferences()
        .Select(m => m.DeclaringType?.Namespace)
        .OfType<string>()
        .Where(ns => ns.Length > 0));

    var data = typeRefs.Concat(memberRefNamespaces).Distinct(StringComparer.OrdinalIgnoreCase);

    foreach (var item in data)
    {
      this.RunRegexMatches(context, item);
      this.RunPlaintextMatching(context, item);
    }
  }
}
