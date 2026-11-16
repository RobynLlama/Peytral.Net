using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyInspect.Observations;
using AssemblyInspect.Scanners.Models;
using Godot;

[GlobalClass]
public partial class SourceCodeEditor : CodeEdit
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
  private IEnumerable<SourceFileObservation> LastInHighlights = [];

  public static SourceCodeEditor Instance;

  public override void _Ready()
  {
    DocumentManager.OnScanCompleted += DocumentScanned;
    DocumentManager.OnDocumentClosed += Clear;
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

  public new void Clear()
  {
    base.Clear();

    //View lines, icons and text
    SetLineBackgroundColor(0, new(0, 0, 0, 0));
    SetLineGutterIcon(0, GutterID, null);

    //search
    SetSearchText(string.Empty);
    SetSearchFlags(0u);
    SearchBar.Text = string.Empty;
    SearchBar.Visible = false;

    //Finally set the text back to placeholder text for highlighting
    Text = PlaceholderText;
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

  public void ChangeToDocument(string documentPath)
  {
    var results = DocumentManager.LastScanResults;
    Clear();

    if (results is null)
      return;

    KeyValuePair<string, string> sentinel = new("INVALID", string.Empty);
    var document = results.SelectMany(x => x.Context.VirtualProject)
      .FirstOrDefault(x => x.Key.Equals(documentPath, StringComparison.OrdinalIgnoreCase), sentinel);

    if (string.IsNullOrEmpty(document.Value))
      return;

    Text = document.Value;

    //find all source file items relevant to the current file
    LastInHighlights = results.SelectMany(x => x.Observations)
      .OfType<SourceFileObservation>()
      .Where(x => x.FilePath.Equals(documentPath, StringComparison.OrdinalIgnoreCase));

    foreach (var item in LastInHighlights)
    {
      GD.Print($"Item Location: {item.Location}");
      var line = item.LineNumber;

      switch (item.Severity)
      {
        case SeverityLevel.Alert:
        case SeverityLevel.Critical:
          SetLineGutterIcon(line, GutterID, AlertTex);
          SetLineBackgroundColor(line, AlertColor);
          break;
        case SeverityLevel.Warning:
          SetLineGutterIcon(line, GutterID, WarnTex);
          SetLineBackgroundColor(line, WarnColor);
          break;
        default:
          SetLineGutterIcon(line, GutterID, MessageTex);
          SetLineBackgroundColor(line, MessageColor);
          break;
      }
    }
  }

  private void DocumentScanned(IEnumerable<ScanResult> results)
  {
    var file = results.SelectMany(x => x.Context.VirtualProject.Keys)
      .FirstOrDefault(x => x.Equals(".csproj", StringComparison.OrdinalIgnoreCase));

    if (string.IsNullOrEmpty(file))
      return;

    ChangeToDocument(file);
  }
}
