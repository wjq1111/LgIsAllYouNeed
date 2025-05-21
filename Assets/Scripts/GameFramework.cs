using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFramework : MonoSingleton<GameFramework>
{
    // Start is called before the first frame update
    void Start()
    {
        // 加载prefab，还需要一套ui管理器来管理
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Start");
        Instantiate(Prefab);

        GameplayActionMgr.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
