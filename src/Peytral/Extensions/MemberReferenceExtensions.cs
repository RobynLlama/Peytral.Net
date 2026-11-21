using Mono.Cecil;

namespace Peytral.Extensions;

public static partial class Extensions
{
  public static string ShortSignature(this MemberReference self)
  {
    var declaringType = self.DeclaringType;
    if (declaringType is GenericInstanceType gen)
      declaringType = gen.ElementType;

    return $"{declaringType.FullName}::{self.Name}";
  }
}
