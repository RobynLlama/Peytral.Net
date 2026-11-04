using System;
using System.Collections.Generic;

namespace AssemblyInspect.Utils;

public static partial class Utilities
{
  public static IEnumerable<int> FindAllOffsets(string source, string value, StringComparison comparison = StringComparison.Ordinal)
  {
    //detect whoopsie moments
    if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
      yield break;

    //step through from the first occurrence, skip if 0 occurrences
    int index = source.IndexOf(value, comparison);
    while (index != -1)
    {
      yield return index;
      //start reading from the end of the prior match
      //potential todo, maybe only advance by 1 rather than item size
      //to find substrings, not sure if that is desirable behavior
      index = source.IndexOf(value, index + value.Length, comparison);
    }
  }
}
