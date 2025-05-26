using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private List<Card> cardList = new List<Card> ();

    // 玩家现在选择了哪一张手牌
    private int chooseCardPosX = -1;
    private int chooseCardPosY = -1;

    // 玩家现在选择了哪个棋盘的格子
    private int chooseBattleFileTilePosX = -1;
    private int chooseBattleFileTilePosY = -1;

    public List<Card> CardList { get => cardList; set => cardList = value; }
    public int ChooseCardPosX { get => chooseCardPosX; set => chooseCardPosX = value; }
    public int ChooseCardPosY { get => chooseCardPosY; set => chooseCardPosY = value; }
    public int ChooseBattleFileTilePosX { get => chooseBattleFileTilePosX; set => chooseBattleFileTilePosX = value; }
    public int ChooseBattleFileTilePosY { get => chooseBattleFileTilePosY; set => chooseBattleFileTilePosY = value; }

    public void InitPlayer()
    {
        
    }

    public void ResetChooseCard()
    {
        ChooseCardPosX = -1;
        ChooseCardPosY = -1;
    }

    public void ResetChooseBattleFieldTile()
    {
        ChooseBattleFileTilePosX = -1;
        ChooseBattleFileTilePosY = -1;
    }

    public void AddCard(List<Card> AddCardList)
    {
        foreach (Card AddCard in AddCardList)
        {
            CardList.Add(AddCard);
        }
    }

    public void DelCard(int PosX, int PosY)
    {
        CardList.RemoveAt(PosX);
    }
}
