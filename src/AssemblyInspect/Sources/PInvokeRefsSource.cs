using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace AssemblyInspect.Sources;

public class PInvokeRefsSource : Source<MethodDefinition>
{
  public override MethodDefinition[] GetSourceData(ScanContext context)
    => [.. context.Assembly.Modules
        .SelectMany(module => module.GetAllTypes())
        .SelectMany(type => type.GetMethods())
        .Where(method => (method.Attributes & MethodAttributes.PInvokeImpl) != 0)];
}
