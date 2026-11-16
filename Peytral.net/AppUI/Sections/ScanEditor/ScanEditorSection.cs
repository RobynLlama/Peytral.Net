using System;
using Godot;

[GlobalClass]
public partial class ScanEditorSection : VBoxContainer
{
  [Export]
  public ScannerElementList ElementList;
  [Export]
  public OptionButton ScannerTypeDropdown;
  [Export, ExportGroup("Buttons")]
  public Button BtnSwitch;
  [Export]
  public Button BtnNewPlain;
  [Export]
  public Button BtnNewRegex;
  [Export]
  public Button BtnSave;
  [Export]
  public Button BtnClear;


  public override void _Ready()
  {
    BtnNewPlain.Pressed += () => ElementList.AddNewElement(false);
    BtnNewRegex.Pressed += () => ElementList.AddNewElement(true);
    BtnClear.Pressed += ElementList.Clear;

    var idx = 0;
    ScannerTypeDropdown.Clear();

    foreach (var item in ScannerManager.EnumerateFactories())
    {
      ScannerTypeDropdown.AddItem(item.Value.FriendlyName, idx);
      ScannerTypeDropdown.SetItemMetadata(idx, item.Key);
      idx++;
    }

    ScannerTypeDropdown.ItemSelected += ScannerTypeSelected;
  }

  private void ScannerTypeSelected(long index)
  {
    var key = ScannerTypeDropdown.GetItemMetadata((int)index);
    ScannerManager.TryCreateScanner(key.AsString(), out var scanner);
    GD.Print($"Selected: {key} : {scanner.GetType().Name}");
  }
}
