using System;
using Godot;

[GlobalClass]
public partial class ScannerPrefab : PanelContainer, IPoolItem
{
  public ScannerData OwnedScanner;

  [Export]
  public Label ScannerName;
  [Export]
  public Label ScannerDesc;
  [Export]
  public Button BtnDelete;

  public void EnterPool()
  {
    Visible = false;
    OwnedScanner = null;
    ScannerName.Text = string.Empty;
    ScannerDesc.Text = string.Empty;
  }


  public void ExitPool() { }
}
