using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners;
using Godot;
using Peytral;

[GlobalClass]
public partial class SourceView : CodeEdit
{

  [Export]
  public LineEdit SearchBar;
  [Export]
  public ExamineViews ExaminePanel;

  [Export, ExportGroup("Highlighting")]
  public Texture2D AlertTex;
  [Export]
  public Color AlertColor;
  [Export]
  public Texture2D WarnTex;
  [Export]
  public Color WarnColor;
  [Export]
  public Texture2D MessageTex;
  [Export]
  public Color MessageColor;

  public int GutterID { get; private set; }
  private ObservationWithLineInfo[] LastInHighlights = [];

  public static SourceView Instance;

  public override void _Ready()
  {
    DocumentManager.OnDocumentScanned += DocumentScanned;
    SetSearchFlags((uint)SearchFlags.WholeWords);
    SearchBar.TextChanged += OnSearchTextChanged;
    SearchBar.TextSubmitted += FindNextString;
    AddGutter();
    //I guess this is how you do gutters?
    //Not like we have concrete types and first-class access or the
    //ability to return an int from AddGutter(), thanks GDScript
    GutterID = GetGutterCount() - 1;
    SetGutterWidth(GutterID, 24);
    SetGutterClickable(GutterID, true);
    SetGutterType(GutterID, GutterType.Icon);
    SetGutterName(GutterID, "ScannerGutter");
    SetGutterDraw(GutterID, true);
    GutterClicked += GutterHandler;

    Instance = this;
  }

  private void GutterHandler(long line, long gutter)
  {
    //Godot garbage nonsense sends the gutter ID as a long but no methods accept longs
    //ditto for line
    var idx = (int)gutter;
    var lineId = (int)line;

    if (idx == GutterID)
    {
      //known issue: this doesn't handle multiple issues on a single line at all
      var item = LastInHighlights.FirstOrDefault(x => x.LineNumber == lineId);
      if (item is null)
        return;

      ExaminePanel.Observation.DisplayObservation(item);
      ExaminePanel.PopupCentered();
    }

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
    //known issue: this doesn't handle multiple issues on a single line at all
    Text = context.FullSourceBlob;
    LastInHighlights = [.. context.Observations.OfType<ObservationWithLineInfo>()];

    foreach (var item in LastInHighlights)
    {

      Texture2D gutterTex;
      Color bColor;

      switch (item.Severity)
      {
        case SeverityLevel.Critical:
        case SeverityLevel.Alert:
          gutterTex = AlertTex;
          bColor = AlertColor;
          break;
        case SeverityLevel.Warning:
          gutterTex = WarnTex;
          bColor = WarnColor;
          break;
        default:
          gutterTex = MessageTex;
          bColor = MessageColor;
          break;
      }

      SetLineBackgroundColor(item.LineNumber, bColor);
      SetLineGutterIcon(item.LineNumber, GutterID, gutterTex);
    }
  }
}
