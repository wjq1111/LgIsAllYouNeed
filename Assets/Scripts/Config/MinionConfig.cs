using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using System.ComponentModel;

[System.Serializable]
public class MinionConfigItem
{
    // ���id����Ҫ�У������ڹؿ��༭����������Ҫ�õ����Ϊkey
    public int MinionId;
    // ����
    public string Name;
    // ������
    public int Attack;
    // ������
    public int Defense;
    // ����ֵ
    public int HitPoint;
    // �����ж���
    public int BaseAction;
    // �����ƶ���
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
