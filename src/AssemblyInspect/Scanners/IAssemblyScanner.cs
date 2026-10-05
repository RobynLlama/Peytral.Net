namespace AssemblyInspect.Scanners;

public interface IAssemblyScanner
{
  public string ScannerName { get; }
  public void Scan(ScanContext context);
}
