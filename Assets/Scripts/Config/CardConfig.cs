using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardConfigItem
{
    // ����id
    public int CardId;
    // ��������
    public string CardName;
    // ����Ч��id
    public List<int> CardEffect;
}

[System.Serializable]
public class CardConfig
{
    public List<CardConfigItem> Cards;
    private bool _mapsInitialize = false;

    private Dictionary<int, CardConfigItem> cardMap = new Dictionary<int, CardConfigItem>();

    public Dictionary<int, CardConfigItem> CardMap { get => cardMap; set => cardMap = value; }

    public void InitMap()
    {
        if (_mapsInitialize)
        {
            return;
        }

        foreach (CardConfigItem item in Cards)
        {
            CardMap.Add(item.CardId, item);
        }
        _mapsInitialize = true;
    }
}
