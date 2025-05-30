using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class AttrEffect : BaseEffect
{
    public EffectAttrRoundType EffectAttrRoundType; // ÿ�غ� ���� һ�غ�
    public int Attack = 0;
    public int Defense = 0;
    public int Hitpoint = 0;
    public int Movement = 0;
    public int Action = 0;

    public void Init(EffectAttrConfig EffectAttrConfig)
    {
        EffectAttrRoundType = (EffectAttrRoundType)EffectAttrConfig.EffectAttrRoundType;
        Attack = EffectAttrConfig.Attack;
        Defense = EffectAttrConfig.Defense;
        Hitpoint = EffectAttrConfig.Hitpoint;
        Movement = EffectAttrConfig.Movement;
        Action = EffectAttrConfig.Action;
    }
}