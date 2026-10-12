using Godot;

public partial class CodeSearchBar : LineEdit
{

  [Export]
  public CodeEdit Editor;

  public override void _GuiInput(InputEvent @event)
  {
    if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
      if ((keyEvent.Keycode == Key.F && keyEvent.IsCommandOrControlPressed()) || keyEvent.Keycode == Key.Escape)
      {
        Visible = false;
        Editor.GrabFocus();
        GetViewport().SetInputAsHandled();
      }

  }
}
