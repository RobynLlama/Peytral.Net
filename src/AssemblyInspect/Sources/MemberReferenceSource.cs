using System.Linq;
using Mono.Cecil;

namespace AssemblyInspect.Sources;

public class MemberReferenceSource : Source<MemberReference>
{
  public override MemberReference[] GetSourceData(ScanContext context)
    => [.. context.Assembly.Modules
      .SelectMany(module => module.GetMemberReferences())];
}
