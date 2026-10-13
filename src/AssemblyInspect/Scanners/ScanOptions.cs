namespace AssemblyInspect;

public record ScanOptions
{
  public string? CacheDirectory { get; init; }
  public bool EnableSourceCache => !string.IsNullOrWhiteSpace(CacheDirectory);
}
