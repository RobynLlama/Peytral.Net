using Mono.Cecil;

namespace AssemblyInspect.Extensions;

public static partial class Extensions
{
  public static string Describe(this MethodReference self)
    => $"{self.DeclaringType.FullName}::{self.Name}";
}
