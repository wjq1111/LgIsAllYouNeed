using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameFramework : MonoSingleton<GameFramework>
{
    private GameObject startPrefab;
    public GameObject StartPrefab { get => startPrefab; set => startPrefab = value; }

    public int CardNumPerRound = 3;

    // Start is called before the first frame update
    void Start()
    {
        // 配置第一个加载
        ConfigMgr.Instance.LoadFromJson();
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

    public void BattleLog(string Log)
    {
        // 追加日志 超过10行的顶掉第一条
        TMP_Text Text = DfsObj(StartPrefab.transform, "BattleLog").GetComponent<TMP_Text>();
        string CurText = Text.text;
        string[] SubStr = CurText.Split('\n');

        int MaxStrLength = 20;
        string Result = "";
        if (SubStr.Length >= MaxStrLength)
        {
            for (int i = 0; i < SubStr.Length - 1; i++)
            {
                Result += SubStr[i + 1] + '\n';
            }
            Result += Log;
        }
        else
        {
            for (int i = 0; i < SubStr.Length; i++)
            {
                Result += SubStr[i] + '\n';
            }
            Result += Log;
        }
        Text.SetText(Result);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
