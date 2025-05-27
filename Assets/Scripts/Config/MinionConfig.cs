using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;

[System.Serializable]
public class MinionConfigItem
{
    public int id;
    public string name;
}

[System.Serializable]
public class MinionConfig
{
    public List<MinionConfigItem> minions;
    private bool _mapsInitialize = false;

    private Dictionary<int, MinionConfigItem> minionConfigMap = new Dictionary<int, MinionConfigItem>();

    public Dictionary<int, MinionConfigItem> MinionConfigMap { get => minionConfigMap; set => minionConfigMap = value; }

    public void InitMap()
    {
        if (_mapsInitialize)
        {
            return;
        }

        foreach (MinionConfigItem item in minions)
        {
            MinionConfigMap.Add(item.id, item);
        }
        _mapsInitialize = true;

        foreach (var item in MinionConfigMap)
        {
            Debug.Log(item.Key + ": " + item.Value.id + " + " + item.Value.name);
        }
    }

}
