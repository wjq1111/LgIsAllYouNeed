using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : Singleton<CardDeck>
{
    private Dictionary<int, Card> allCards = new Dictionary<int, Card>();

    // value = cardId
    private List<Card> cards = new List<Card>();

    public new void Init()
    {
        // TODO 根据配置加进牌库

        List<BaseEffect> effects = new List<BaseEffect>();
        SummonEffect summonEffect = new SummonEffect();

        Card NewCard1 = new Card();
        NewCard1.Gid = 0;

        summonEffect.Init(2, 1, 1);
        effects.Add(summonEffect);
        NewCard1.Init("Gaowenchang", effects);
        effects.Clear();

        Card NewCard2 = new Card();
        NewCard2.Gid = 1;

        summonEffect.Init(1, 2, 1);
        effects.Add(summonEffect);
        NewCard2.Init("Wenchanggao", effects);
        effects.Clear();

        Card NewCard3 = new Card();
        NewCard3.Gid = 2;

        summonEffect.Init(1, 1, 2);
        effects.Add(summonEffect);
        NewCard3.Init("Changgaowen", effects);
        effects.Clear();

        Card NewCard4 = new Card();
        NewCard4.Gid = 3;

        summonEffect.Init(2, 2, 2);
        effects.Add(summonEffect);
        NewCard4.Init("MachineGun", effects);
        effects.Clear();

        allCards.Add(NewCard1.Gid, NewCard1);
        allCards.Add(NewCard2.Gid, NewCard2);
        allCards.Add(NewCard3.Gid, NewCard3);
        allCards.Add(NewCard4.Gid, NewCard4);

        // TODO 读取初始牌库
        cards.Add(NewCard1);
        cards.Add(NewCard2);
        cards.Add(NewCard3);
        cards.Add(NewCard4);
    }

    public List<Card> DrawCards(int Num = 1)
    {
        // 随机num张，并删除
        List<Card> ResultCards = new List<Card>();
        for(int i = 0; i < Num; i++)
        {
            int Index = (int)(Random.value * cards.Count);
            Debug.Log("random index " + Index + " " + cards.Count);
            Card ChooseCard = cards[Index];
            ResultCards.Add(ChooseCard);
            cards.Remove(ChooseCard);
        }

        return ResultCards;
    }
}
