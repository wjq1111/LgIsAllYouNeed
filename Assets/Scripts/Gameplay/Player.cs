using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private List<Card> cardList = new List<Card> ();

    public List<Card> CardList { get => cardList; set => cardList = value; }

    public void InitPlayer()
    {
        
    }

    public void AddCard(List<Card> AddCardList)
    {
        foreach (Card AddCard in AddCardList)
        {
            CardList.Add(AddCard);
        }

        // 再次渲染自己的手牌

    }
}
