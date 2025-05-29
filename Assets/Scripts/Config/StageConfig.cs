using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using System.ComponentModel;

[System.Serializable]
public class EnemyMinion
{
    public int MinionId;
    public int TilePosX;
    public int TilePosY;
}

[System.Serializable]
public class StageConfigItem
{
    // �ؿ�id
    public int StageId;
    // ��һ�ص�boss��
    public int BossMinionId;
    // boss�Ĺ�������
    public int TilePosX;
    public int TilePosY;
    public List<EnemyMinion> EnemyMinions;
}

[System.Serializable]
public class StageConfig
{
    public List<StageConfigItem> stages;
    private bool _mapsInitialize = false;

    private Dictionary<int, StageConfigItem> stageConfigMap = new Dictionary<int, StageConfigItem>();

    public Dictionary<int, StageConfigItem> StageConfigMap { get => stageConfigMap; set => stageConfigMap = value; }

    public void InitMap()
    {
        if (_mapsInitialize)
        {
            return;
        }

        foreach (StageConfigItem item in stages)
        {
            StageConfigMap.Add(item.StageId, item);
        }
        _mapsInitialize = true;
    }

}
