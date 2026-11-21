using System;
using Godot;

[GlobalClass]
public partial class HelpWindow : Window
{
  [Export]
  public Button BtnClose;
  [Export]
  public RichTextLabel TextArea;

  public override void _Ready()
  {
    BtnClose.Pressed += () => Visible = false;
  }
}
