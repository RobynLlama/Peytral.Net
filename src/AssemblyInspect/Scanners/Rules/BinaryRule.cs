using System;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners.Rules;

public class BinaryRule(byte[] pattern, string summary, string analysis, SeverityLevel severity)
{
  private readonly byte[] _pattern = pattern;
  public ReadOnlySpan<byte> Pattern => _pattern;
  public string Summary = summary;
  public string Analysis = analysis;
  public SeverityLevel Severity = severity;

  public bool MatchesSequence(ReadOnlySpan<byte> input)
  {
    //if the input is at least as long as the magic
    //and they are exact sequence equal
    return
      input.Length >= Pattern.Length &&
      input[..Pattern.Length].SequenceEqual(Pattern);
  }
}
