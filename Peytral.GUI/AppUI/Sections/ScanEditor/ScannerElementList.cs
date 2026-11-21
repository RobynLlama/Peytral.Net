using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class ScannerElementList : VBoxContainer
{
  [Export]
  public PackedScene ElementPrefab;
  private ItemPool<ScanElementPrefab> PrefabPool;
  public override void _Ready()
  {
    PrefabPool = new(PoolConstructor);
    PrefabPool.SetMinCapacity(20);
  }

  public void AddNewElement(bool regex = false)
  {
    var item = PrefabPool.Checkout();
    item.IsRegex = regex;
    MoveChild(item, -1);
    item.Visible = true;
  }

  public IEnumerable<PlainTextRuleResource> GetPlainTextRules() =>
     PrefabPool.GetActiveItems.Where(x => !x.IsRegex).Select(x => x.CreatePlainTextResource());

  public IEnumerable<RegexRuleResource> GetRegexRules() =>
    PrefabPool.GetActiveItems.Where(x => x.IsRegex).Select(x => x.CreateRegexResource());

  public void Clear() => PrefabPool.ReturnAllToPool();
  private ScanElementPrefab PoolConstructor()
  {
    var child = ElementPrefab.Instantiate<ScanElementPrefab>();
    child.Visible = false;
    child.OnRemovePressed += PrefabPool.ReturnToPool;
    AddChild(child);
    return child;
  }
}
