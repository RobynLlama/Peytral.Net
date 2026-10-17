using System.Diagnostics.CodeAnalysis;

namespace AssemblyInspect.Scanners;

public record ScanOptions
{
  public string? CacheDirectory { get; init; }

  [MemberNotNullWhen(true, nameof(CacheDirectory))]
  public bool EnableSourceCache => !string.IsNullOrWhiteSpace(CacheDirectory);
}
