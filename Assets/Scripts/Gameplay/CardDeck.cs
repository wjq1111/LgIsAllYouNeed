using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : Singleton<CardDeck>
{
    private Dictionary<int, Card> allCards = new Dictionary<int, Card>();

    public new void Init()
    {
        // TODO 根据配置加进牌库
        Card NewCard1 = new Card();
        NewCard1.Name = "lg 0 card";
        NewCard1.Gid = 0;

        Card NewCard2 = new Card();
        NewCard2.Name = "lg 1 card";
        NewCard2.Gid = 1;

        Card NewCard3 = new Card();
        NewCard3.Name = "lg 2 card";
        NewCard3.Gid = 2;

        allCards.Add(NewCard1.Gid, NewCard1);
        allCards.Add(NewCard2.Gid, NewCard2);
        allCards.Add(NewCard3.Gid, NewCard3);
    }

    public List<Card> DrawCards(int Num = 1)
    {
        // 随机num张，并删除
        List<Card> ResultCards = new List<Card>();

        int Index = (int)Random.value * allCards.Count;
        Debug.Log("random index " + Index + " " + allCards.Count);
        Card ChooseCard = allCards[Index];
        ResultCards.Add(ChooseCard);
        allCards.Remove(ChooseCard.Gid);

        return ResultCards;
    }
}
