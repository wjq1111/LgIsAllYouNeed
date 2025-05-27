using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 牌库，本次单局内发牌员用的牌库，gid在DrawCard之后真正被实例化赋值
 */
public class CardDeck : Singleton<CardDeck>
{
    private List<Card> cards = new List<Card>();

    private List<Card> usedCards = new List<Card>();

    public List<Card> Cards { get => cards; set => cards = value; }
    public List<Card> UsedCards { get => usedCards; set => usedCards = value; }

    public void Init(List<Card> CardList, int PlayerId)
    {
        // 加入player的已解锁的牌
        foreach (Card MyCard in CardList)
        {
            MyCard.PlayerId = PlayerId;
            Cards.Add(MyCard);
        }
    }

    public List<Card> DrawCards(int Num = 1)
    {
        // 随机num张，并删除
        List<Card> ResultCards = new List<Card>();
        for (int i = 0; i < Num; i++)
        {
            if (Cards.Count == 0)
            {
                foreach (Card UsedCard in UsedCards)
                {
                    Cards.Add(UsedCard);
                }
                UsedCards.Clear();
            }

            int Index = (int)(Random.value * Cards.Count);
            Debug.Log("random index " + Index + " remain card count " + Cards.Count);
            Card ChooseCard = Cards[Index];
            ResultCards.Add(ChooseCard);
            Cards.Remove(ChooseCard);

            UsedCards.Add(ChooseCard);
        }

        return ResultCards;
    }
}
