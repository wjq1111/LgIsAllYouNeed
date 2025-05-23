using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFramework : MonoSingleton<GameFramework>
{
    private GameObject startPrefab;
    public GameObject StartPrefab { get => startPrefab; set => startPrefab = value; }

    public int CardNumPerRound = 3;

    // Start is called before the first frame update
    void Start()
    {
        // 加载prefab，还需要一套ui管理器来管理
        StartPrefab = Instantiate((GameObject)Resources.Load("Prefabs/Start"));

        GameplayActionMgr.Instance.Init();
        Handbook.Instance.Init();
    }

    public static Transform DfsObj(Transform Transform, string TargetName)
    {
        Transform TargetTransform = Transform.Find(TargetName);
        if (TargetTransform != null)
        {
            return TargetTransform;
        }

        for (int i = 0; i < Transform.childCount; i++)
        {
            TargetTransform = DfsObj(Transform.GetChild(i), TargetName);
            if (TargetTransform != null)
            {
                return TargetTransform;
            }
        }
        return null;
    }

    public GameObject GetBattleFieldObj()
    {
        return DfsObj(StartPrefab.transform, "BattleField").gameObject;
    }

    public GameObject GetCardSessionObj()
    {
        return DfsObj(StartPrefab.transform, "CardSession").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
