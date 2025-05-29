using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHandbookConfigItem
{
    // ≥ı º≈∆ø‚ø®≈∆id
    public int CardId;
}

[System.Serializable]
public class BaseHandbookConfig
{
    public List<BaseHandbookConfigItem> Cards;
    private bool _mapsInitialize = false;

    private Dictionary<int, BaseHandbookConfigItem> cardMap = new Dictionary<int, BaseHandbookConfigItem>();

    public Dictionary<int, BaseHandbookConfigItem> CardMap { get => cardMap; set => cardMap = value; }

    public void InitMap()
    {
        if (_mapsInitialize)
        {
            return;
        }

        foreach (BaseHandbookConfigItem item in Cards)
        {
            CardMap.Add(item.CardId, item);
        }
        _mapsInitialize = true;
    }
}