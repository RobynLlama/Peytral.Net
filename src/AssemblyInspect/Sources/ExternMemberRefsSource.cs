using System.Linq;
using Mono.Cecil;

namespace AssemblyInspect.Sources;

public class ExternMemberRefsSource : Source<MethodReference>
{
  public override MethodReference[] GetSourceData(ScanContext context)
    => [.. context.Assembly.Modules
      .SelectMany(module => module.GetMemberReferences())
      .OfType<MethodReference>()];
}
