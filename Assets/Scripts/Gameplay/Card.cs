using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 ��Ƭid���������ʲô����Ŀ�������˵�ٻ�һ��1/1���ٻ�һ��2/3����1��
 ��Ƭgid��������ڱ��ζԾ��е�ʵ�������id������˵�����ٻ���һ��1/1������gid��1��Ȼ�����ٻ���һ��1/1������gid��2
 �����ҳ���gidΪ1��2��3���Է���4��5��6���Ҵ��1���ֳ���һ��7���ҵ����Ʊ����2��3��7
 ��Ȼ����gid��ͬ�����������2��3��7����ͬʱΪ�ٻ�һ��1/1
 */

public class Card
{
    private int playerId = 0;

    private int cardId = 0;
    private int gid = 0;
    private string name = "default card";

    // ���ſ���ʲôЧ���������ٻ�Ч���������buffЧ��
    private List<BaseEffect> cardEffect = new List<BaseEffect>();

    public int CardId { get => cardId; set => cardId = value; }
    public int Gid { get => gid; set => gid = value; }
    public string Name { get => name; set => name = value; }
    public List<BaseEffect> CardEffect { get => cardEffect; set => cardEffect = value; }
    public int PlayerId { get => playerId; set => playerId = value; }

    public void Init(CardConfigItem CardConfigItem)
    {
        CardId = CardConfigItem.CardId;
        Name = CardConfigItem.CardName;

        foreach (int EffectId in CardConfigItem.CardEffect)
        {
            EffectConfigItem EffectConfigItem = ConfigMgr.Instance.EffectConfig.EffectMap[EffectId];
            if (EffectConfigItem.EffectType == (int)EffectType.EffectType_Summon)
            {
                SummonEffect SummonEffect = new SummonEffect();
                SummonEffect.EffectType = EffectType.EffectType_Summon;

                MinionConfigItem MinionConfigItem = ConfigMgr.Instance.MinionConfig.MinionConfigMap[EffectConfigItem.MinionId];
                SummonEffect.Init(MinionConfigItem);

                CardEffect.Add(SummonEffect);
            }
            else
            {
                // TODO ���࿨Ч��
            }
        }
    }
}
