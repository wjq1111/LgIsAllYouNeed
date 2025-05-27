using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 �ƿ⣬���ε����ڷ���Ա�õ��ƿ⣬gid��DrawCard֮��������ʵ������ֵ
 */
public class CardDeck : Singleton<CardDeck>
{
    private List<Card> cards = new List<Card>();

    private List<Card> usedCards = new List<Card>();

    public List<Card> Cards { get => cards; set => cards = value; }
    public List<Card> UsedCards { get => usedCards; set => usedCards = value; }

    public void Init(List<Card> CardList, int PlayerId)
    {
        // ����player���ѽ�������
        foreach (Card MyCard in CardList)
        {
            MyCard.PlayerId = PlayerId;
            Cards.Add(MyCard);
        }
    }

    public List<Card> DrawCards(int Num = 1)
    {
        // ���num�ţ���ɾ��
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
