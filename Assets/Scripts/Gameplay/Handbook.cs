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
        // TODO 配置 第一次开游戏，肯定需要一些基础牌
        UnlockedCards.Add(InitTempCard(1));
        UnlockedCards.Add(InitTempCard(2));
        UnlockedCards.Add(InitTempCard(3));
        UnlockedCards.Add(InitTempCard(4));
        UnlockedCards.Add(InitTempCard(5));

    }

    private Card InitTempCard(int Index)
    {
        Card StartCard = new Card();
        StartCard.CardId = Index;
        StartCard.Name = "a" + Index + "-d1-f1";
        StartCard.CardEffect = new List<BaseEffect>();
        SummonEffect StartCardSummonEffect = new SummonEffect();
        StartCard.CardEffect.Add(StartCardSummonEffect);
        return StartCard;
    }
}