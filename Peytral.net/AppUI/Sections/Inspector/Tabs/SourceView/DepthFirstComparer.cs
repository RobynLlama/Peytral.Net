using System;
using System.Collections.Generic;

/// <summary>
/// This should sort folders ahead of files to make the
/// tree list look a lot cleaner. I kinda hate comparers tho
/// </summary>
public class DepthFirstComparer : IComparer<string>
{
  public int Compare(string x, string y)
  {
    string[] xParts = x.Split('/');
    string[] yParts = y.Split('/');

    //only compare up to the smaller of the two
    var min = Math.Min(xParts.Length, yParts.Length);

    //search up the path until we find the part that is different
    //then sort by if those parts are directories or files
    for (int i = 0; i < min; i++)
    {
      if (xParts[i] != yParts[i])
      {
        //Whichever one (or both) is on its final item is
        //always a file because my flat pathFS doesn't support
        //entries that are just directories
        bool xIsFile = i == xParts.Length - 1;
        bool yIsFile = i == yParts.Length - 1;

        //Folders (if any) win
        if (xIsFile && !yIsFile) return 1;
        if (!xIsFile && yIsFile) return -1;

        //fall back to alphabetically if both are folders / files
        //ignore case, etc
        return string.Compare(xParts[i], yParts[i], StringComparison.OrdinalIgnoreCase);
      }
    }

    //This could only be reached if the shorter path is
    //an EXACT prefix of the other path, which can't happen
    //cuz we don't support directory entires but the compiler
    //NEEDS something here for completeness sake
    //So return the comparison of the size of the parts
    return xParts.Length.CompareTo(yParts.Length);
  }
}
