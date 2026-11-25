using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Peytral.Observations;

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

  public void AddNewElement(bool regex, string summary, string pattern, string description, SeverityLevel severity)
  {
    var item = PrefabPool.Checkout();
    item.IsRegex = regex;
    item.SummaryInput.Text = summary;
    item.PatternInput.Text = pattern;
    item.MessageInput.Text = description;
    item.SeverityInput.Selected = (int)severity;
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
    child.BtnDelete.Pressed += () => PrefabPool.ReturnToPool(child);
    AddChild(child);
    return child;
  }
}
