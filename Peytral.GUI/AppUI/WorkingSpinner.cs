using System;
using Godot;

public partial class WorkingSpinner : TextureRect
{
  public override void _Ready()
  {
    PivotOffset = new(48, 48);
  }

  public override void _Process(double delta)
  {
    Rotation += 0.05f;
  }
}
