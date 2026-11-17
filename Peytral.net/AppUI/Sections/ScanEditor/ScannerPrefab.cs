using System;
using Godot;

[GlobalClass]
public partial class ScannerPrefab : MarginContainer, IPoolItem
{
  public ScannerData OwnedScanner;

  [Export]
  public Label ScannerName;
  [Export]
  public Button BtnDelete;

  public void EnterPool()
  {
    Visible = false;
    OwnedScanner = null;
    ScannerName.Text = string.Empty;
  }


  public void ExitPool() { }
}
