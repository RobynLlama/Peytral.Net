using AssemblyInspect.Scanners;
using Godot;

[GlobalClass]
public abstract partial class ScannerData : Resource
{
  public abstract IAssemblyScanner BuildScanner();
}
