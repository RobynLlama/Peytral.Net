using Mono.Cecil;

namespace Peytral.Extensions;

public static partial class Extensions
{
  public static string Describe(this MethodDefinition self)
    => $"{self.DeclaringType.FullName}::{self.Name}";

  public static string DescribePInvoke(this MethodDefinition self)
  {
    if (!self.HasPInvokeInfo)
      return self.Describe();

    return $"{self.Describe} → {self.PInvokeInfo.Module.Name} ({self.PInvokeInfo.EntryPoint})";
  }
}
