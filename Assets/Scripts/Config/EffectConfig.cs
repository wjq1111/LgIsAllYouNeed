using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectAttrConfig
{
    public int EffectAttrRoundType; // ÿ�غ� ���� һ�غ�
    public int Attack;
    public int Defense;
    public int Hitpoint;
    public int Movement;
    public int Action;
}

[System.Serializable]
public class EffectConfigItem
{
    // Ч��id
    public int EffectId;
    // Ч������
    public int EffectType;
    // EffectType = 1 �ٻ�Ч�� ��Ӧ���ٻ���id
    public int MinionId;
    // EffectType = 2 �޸��������
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
