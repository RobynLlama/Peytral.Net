using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace AssemblyInspect.Scanners;

public class PInvokeScanner : IRegexMatcher, IPlaintextMatcher
{

  public string ScannerName => "P/Invoke Scanner";
  public IEnumerable<RegexRule> RegexRules { get; set; } = [];
  public IEnumerable<PlainTextRule> PlainTextRules { get; set; } = [];

  public IReadOnlyCollection<Observation> Scan(ScanContext context)
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

      this.RunRegexMatches(obs, key);
      this.RunPlaintextMatching(obs, key);
    }

    return obs;
  }
}
