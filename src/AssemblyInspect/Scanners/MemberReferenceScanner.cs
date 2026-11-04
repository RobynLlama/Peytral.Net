using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Extensions;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;

namespace AssemblyInspect.Scanners;

public class MemberReferenceScanner : SimpleScannerBase
{

  public override string ScannerName => "Member Reference Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public override IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    var data = context.Assembly.Modules
      .SelectMany(module => module.GetMemberReferences());

    List<Observation> obs = [];

    foreach (var item in data)
    {
      var name = item.ShortSignature();
      RunPlainTextMatches(obs, PlainTextRules, name, "Member References");
      RunRegexMatches(obs, RegexRules, name, "Member References");
    }

    return obs;
  }
}
