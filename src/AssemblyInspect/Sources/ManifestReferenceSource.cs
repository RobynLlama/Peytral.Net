using System;
using System.Linq;

namespace AssemblyInspect.Sources;

public class ManifestReferenceSource : Source<string>
{
  public override string[] GetSourceData(ScanContext context)
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

    return [.. typeRefs.Concat(memberRefNamespaces).Distinct(StringComparer.OrdinalIgnoreCase)];
  }
}
