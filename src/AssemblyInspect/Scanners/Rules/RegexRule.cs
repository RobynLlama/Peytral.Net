using System;
using System.Text.RegularExpressions;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners.Rules;

public class RegexRule(string pattern, string summary, string description, SeverityLevel severity)
{
  public readonly string Summary = summary;
  public readonly string Description = description;
  public readonly SeverityLevel Severity = severity;
  public readonly Regex CompiledRegex = new(pattern, RegexOptions.Compiled | RegexOptions.Multiline, TimeSpan.FromSeconds(1));

  /// <summary>
  /// Mostly just a shortcut to try/catch the timeout
  /// Uses IsMatch for simple presence check, does not know
  /// where in the document the match is found
  /// </summary>
  /// <param name="input">The input string to match against</param>
  /// <returns></returns>
  public bool Evaluate(string input)
  {
    try
    {
      if (CompiledRegex.IsMatch(input))
      {
        return true;
      }
    }
    catch (RegexMatchTimeoutException) { }

    return false;
  }
}
