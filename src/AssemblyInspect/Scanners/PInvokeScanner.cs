using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using AssemblyInspect.Scanners.Rules;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace AssemblyInspect.Scanners;

public class PInvokeScanner : SimpleScannerBase
{
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public PInvokeScanner()
  {
    ScannerName = "P/Invoke Scanner";
  }

  public override IReadOnlyCollection<Observation> Scan(ScanContext context)
  {
    bool genericWarning = false;
    var data = context.Assembly.Modules
      .SelectMany(module => module.GetAllTypes())
      .SelectMany(type => type.GetMethods())
      .Where(method => (method.Attributes & MethodAttributes.PInvokeImpl) != 0);

    List<Observation> obs = [];

    foreach (var item in data)
    {
      if (!genericWarning)
      {
        genericWarning = true;
        obs.Add(new(ScannerName, SeverityLevel.Warning, "Module loads native libraries", "The given module loads native libraries. There are some legitimate reasons for this but any native library use should be scrutinized.", "Assembly Metadata"));
      }

      string key = $"{item.PInvokeInfo.Module.Name}::{item.PInvokeInfo.EntryPoint}";

      RunPlainTextMatches(obs, PlainTextRules, key, "P/Invoke References");
      RunRegexMatches(obs, RegexRules, key, "P/Invoke References");
    }

    return obs;
  }
}
