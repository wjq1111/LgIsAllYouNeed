using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectAttrConfig
{
    public int EffectAttrRoundType; // 每回合 还是 一回合
    public int Attack;
    public int Defense;
    public int Hitpoint;
    public int Movement;
    public int Action;
}

[System.Serializable]
public class EffectConfigItem
{
    // 效果id
    public int EffectId;
    // 效果类型
    public int EffectType;
    // EffectType = 1 召唤效果 对应的召唤物id
    public int MinionId;
    // EffectType = 2 修改属性相关
    public EffectAttrConfig EffectAttrConfig;
}

[System.Serializable]
public class EffectConfig
{
    public List<EffectConfigItem> Effects;
    private bool _mapsInitialize = false;

    private Dictionary<int, EffectConfigItem> effectMap = new Dictionary<int, EffectConfigItem>();

    public Dictionary<int, EffectConfigItem> EffectMap { get => effectMap; set => effectMap = value; }

    public void InitMap()
    {
        if (_mapsInitialize)
        {
            return;
        }

        foreach (EffectConfigItem item in Effects)
        {
            EffectMap.Add(item.EffectId, item);
        }
        _mapsInitialize = true;
    }
}
