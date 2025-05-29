using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/*
 ����Ѿ���������
 */

public class Handbook : Singleton<Handbook>
{
    // ����Ⱳ���Ѿ���������
    private List<Card> unlockedCards = new List<Card>();

    public List<Card> UnlockedCards { get => unlockedCards; set => unlockedCards = value; }

    public new void Init()
    {
        // TODO ��Ӧ��ÿ�ζ������ã�����ֻ�е�һ�ζ����ã�������浵
        foreach (var item in ConfigMgr.Instance.BaseHandbookConfig.CardMap)
        {
            Card NewCard = new Card();
            CardConfigItem CardConfigItem = ConfigMgr.Instance.CardConfig.CardMap[item.Value.CardId];
            NewCard.Init(CardConfigItem);
            UnlockedCards.Add(NewCard);
        }
    }
}