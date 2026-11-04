using System;
using System.Collections.Generic;

namespace AssemblyInspect.Scanners.Mapping;

public class DocumentLineMap(string text)
{
  public readonly List<int> LineOffsets = GetLineStartOffsets(text);

  public int GetLineIndex(int charIndex)
  {
    //this could be a one-liner but it gets way too ugly
    int result = LineOffsets.BinarySearch(charIndex);
    return result >= 0 ? result : ~result - 1;
  }

  protected static List<int> GetLineStartOffsets(string text)
  {
    //line 1 is always at 0, duh
    var lineStarts = new List<int> { 0 };
    ReadOnlySpan<char> span = text.AsSpan();

    int offset = 0;

    while (offset < span.Length)
    {

      //this always works for ICSharp.Decompiler since it normalizes
      //line returns to \n exclusively
      int nextIndex = span[offset..].IndexOf('\n');
      if (nextIndex == -1)
        break;

      //add one to account for the \n itself, oops
      offset += nextIndex + 1;
      lineStarts.Add(offset);
    }

    return lineStarts;
  }
}
