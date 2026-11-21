using System.Collections.Generic;
using System.Linq;
using Peytral.Extensions;
using Peytral.Observations;
using Peytral.Scanners.Models;
using Peytral.Scanners.Rules;

namespace Peytral.Scanners;

public class MemberReferenceScanner : SimpleScannerBase
{
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public MemberReferenceScanner()
  {
    ScannerName = "Member Reference Scanner";
  }

  public override IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    var data = context.Assembly.Modules
      .SelectMany(module => module.GetMemberReferences());

    List<Observation> obs = [];
    var location = $"{context.Assembly.Name.Name} assembly metadata";

    foreach (var item in data)
    {
      var name = item.ShortSignature();
      RunPlainTextMatches(obs, PlainTextRules, name, location);
      RunRegexMatches(obs, RegexRules, name, location);
    }

    return obs;
  }
}
