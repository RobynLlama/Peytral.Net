using System.Linq;
using Mono.Cecil;

namespace AssemblyInspect.Sources;

public class ExternMemberRefsSource : Source<MethodReference>
{
  public override MethodReference[] GetSourceData(AssemblyDefinition assembly)
    => [.. assembly.Modules
      .SelectMany(module => module.GetMemberReferences())
      .OfType<MethodReference>()];
}
