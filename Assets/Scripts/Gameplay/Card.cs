using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 卡片id：这个卡是什么具体的卡。比如说召唤一个1/1，召唤一个2/3，加1攻
 卡片gid：这个卡在本次对决中的实例化后的id，比如说我先召唤了一个1/1，他的gid是1，然后又召唤了一个1/1，他的gid是2
 开局我抽了gid为1，2，3，对方是4，5，6，我打出1，又抽了一张7，我的手牌变成了2，3，7
 虽然手牌gid不同，但是这里的2，3，7可以同时为召唤一个1/1
 */

public class Card
{
    private int cardId = 0;
    private int gid = 0;
    private string name = "default card";

    // 这张卡有什么效果，比如召唤效果，比如加buff效果
    private List<BaseEffect> cardEffect = new List<BaseEffect>();

    public int CardId { get => cardId; set => cardId = value; }
    public int Gid { get => gid; set => gid = value; }
    public string Name { get => name; set => name = value; }
    public List<BaseEffect> CardEffect { get => cardEffect; set => cardEffect = value; }

    public void Init(string Name, List<BaseEffect> cardEffect)
    {
        //this.cid = cid;
        this.Name = Name;
        this.CardEffect = cardEffect;
    }
}
