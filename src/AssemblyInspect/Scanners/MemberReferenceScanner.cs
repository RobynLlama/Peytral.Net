using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Extensions;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public class MemberReferenceScanner : IRegexMatcher, IPlaintextMatcher
{

  public string ScannerName => "Member Reference Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public void Scan(ScanContext context)
  {
    var data = context.Assembly.Modules
      .SelectMany(module => module.GetMemberReferences());

    foreach (var item in data)
    {
      var name = item.ShortSignature();
      this.RunRegexMatches(context, name);
      this.RunPlaintextMatching(context, name);
    }
  }
}
