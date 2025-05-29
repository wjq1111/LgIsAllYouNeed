using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using System.ComponentModel;

[System.Serializable]
public class MinionConfigItem
{
    // 随从id，需要有，比如在关卡编辑的配置中需要用到这个为key
    public int MinionId;
    // 名字
    public string Name;
    // 攻击力
    public int Attack;
    // 防御力
    public int Defense;
    // 生命值
    public int HitPoint;
    // 基础行动力
    public int BaseAction;
    // 基础移动力
    public int BaseMovement;
}

[System.Serializable]
public class MinionConfig
{
    public List<MinionConfigItem> Minions;
    private bool _mapsInitialize = false;

    private Dictionary<int, MinionConfigItem> minionConfigMap = new Dictionary<int, MinionConfigItem>();

    public Dictionary<int, MinionConfigItem> MinionConfigMap { get => minionConfigMap; set => minionConfigMap = value; }

    public void InitMap()
    {
        if (_mapsInitialize)
        {
            return;
        }

        foreach (MinionConfigItem item in Minions)
        {
            MinionConfigMap.Add(item.MinionId, item);
        }
        _mapsInitialize = true;
    }
}
