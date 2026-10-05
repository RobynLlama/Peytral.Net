using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using AssemblyInspect.Observations;

namespace AssemblyInspect.Scanners;

public abstract class RegexScannerBase : IAssemblyScanner
{
  public readonly List<RegexRule> RegexRules = [];
  public abstract string ScannerName { get; }
  public void RunAllRules(ScanContext context, string input)
  {
    foreach (var item in RegexRules)
    {
      if (item.Evaluate(input))
        context.Observations.Add(new(ScannerName, item.Severity, item.Summary, item.Description, string.Empty));
    }
  }
  public abstract void Scan(ScanContext context);
}

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
