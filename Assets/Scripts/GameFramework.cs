using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFramework : MonoSingleton<GameFramework>
{
    // Start is called before the first frame update
    void Start()
    {
        // ����prefab������Ҫһ��ui������������
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/Start");
        Instantiate(Prefab);

        GameplayActionMgr.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
