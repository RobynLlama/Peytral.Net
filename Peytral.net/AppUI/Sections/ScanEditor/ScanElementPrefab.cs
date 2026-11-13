using System;
using Godot;

[GlobalClass]
public partial class ScanElementPrefab : VBoxContainer
{
  public bool IsRegex = false;

  [Export]
  public Label LblTitle;
  [Export]
  public Button BtnDelete;
  public event Action<ScanElementPrefab> OnRemovePressed;

  public override void _Ready()
  {
    VisibilityChanged += UpdateTitle;
    BtnDelete.Pressed += () => OnRemovePressed?.Invoke(this);
  }

  private void UpdateTitle()
  {
    if (!Visible)
      return;

    LblTitle.Text = IsRegex ? "Regex Element" : "PlainText Element";
  }

  public void SentToPool()
  {
    Visible = false;
  }
}
