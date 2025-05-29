using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/*
 玩家已经解锁的牌
 */

public class Handbook : Singleton<Handbook>
{
    // 玩家这辈子已经解锁的牌
    private List<Card> unlockedCards = new List<Card>();

    public List<Card> UnlockedCards { get => unlockedCards; set => unlockedCards = value; }

    public new void Init()
    {
        // TODO 不应该每次都读配置，而是只有第一次读配置，后面读存档
        foreach (var item in ConfigMgr.Instance.BaseHandbookConfig.CardMap)
        {
            Card NewCard = new Card();
            CardConfigItem CardConfigItem = ConfigMgr.Instance.CardConfig.CardMap[item.Value.CardId];
            NewCard.Init(CardConfigItem);
            UnlockedCards.Add(NewCard);
        }
    }
}