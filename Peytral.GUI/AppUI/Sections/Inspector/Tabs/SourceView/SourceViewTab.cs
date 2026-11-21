using Godot;

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
    HideButton.Pressed += ToggleDocumentTree;
  }

  private void ToggleDocumentTree()
  {
    TreePanel.Visible = !TreePanel.Visible;
    HideButton.Text = TreePanel.Visible ? "←" : "→";
  }
}
