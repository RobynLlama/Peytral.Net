using System;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

public partial class SourceView : CodeEdit
{

  [Export]
  public LineEdit SearchBar;

  public override void _Ready()
  {
    DocumentManager.OnDocumentScanned += DocumentScanned;
    SetSearchFlags((uint)SearchFlags.WholeWords);
    SearchBar.TextChanged += OnSearchTextChanged;
    SearchBar.TextSubmitted += FindNextString;
  }

  public override void _GuiInput(InputEvent @event)
  {
    if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
    {
      if (keyEvent.Keycode == Key.F && keyEvent.IsCommandOrControlPressed())
      {
        SearchBar.Visible = !SearchBar.Visible;

        if (SearchBar.Visible)
          SearchBar.GrabFocus();

        GetViewport().SetInputAsHandled();
      }
    }
  }

  private void OnSearchTextChanged(string newText)
  {
    //This just sets the highlighting, searching is only
    //done when the search bar is submitted

    SetSearchText(newText);
    SetSearchFlags(0);
    QueueRedraw();
  }

  public void FindNextString(string text)
  {
    //backspaced the last character, etc
    if (string.IsNullOrEmpty(text)) return;

    var flags = (uint)SearchFlags.WholeWords;

    //actually search from the caret
    Vector2I result = Search(text, flags, GetCaretLine(), GetCaretColumn());

    //wrap to the start of the document automatically
    if (result.X == -1)
    {
      result = Search(text, flags, 0, 0);
    }

    //if we found anything (including wrapping)
    if (result.X != -1)
    {
      Select(result.Y, result.X, result.Y, result.X + text.Length);
      SetCaretLine(result.Y);
      SetCaretColumn(result.X + text.Length);
    }
  }

  private void DocumentScanned(ScanContext context)
  {
    Text = context.FullSourceBlob;
  }
}
