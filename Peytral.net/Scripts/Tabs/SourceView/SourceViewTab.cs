using System;
using Godot;
using Peytral;

public partial class SourceViewTab : MarginContainer
{
  [Export]
  public SourceCodeEditor SourceEditor;
  [Export]
  public VBoxContainer TreePanel;
  [Export]
  public Button HideButton;

  public override void _Ready()
  {
    DocumentManager.OnDocumentClosed += ResetTab;
    HideButton.Pressed += ToggleDocumentTree;
  }

  private void ToggleDocumentTree()
  {
    TreePanel.Visible = !TreePanel.Visible;
    HideButton.Text = TreePanel.Visible ? "←" : "→";
  }

  private void ResetTab()
  {
    //View lines, icons and text
    SourceEditor.Text = string.Empty;
    SourceEditor.SetLineBackgroundColor(0, new(0, 0, 0, 0));
    SourceEditor.SetLineGutterIcon(0, SourceEditor.GutterID, null);

    //search
    SourceEditor.SetSearchText(string.Empty);
    SourceEditor.SetSearchFlags(0u);
    SourceEditor.SearchBar.Text = string.Empty;
    SourceEditor.SearchBar.Visible = false;

    //Finally set the text back to placeholder text for highlighting
    SourceEditor.Text = SourceEditor.PlaceholderText;
  }
}
