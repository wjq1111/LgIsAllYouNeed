using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldBehaviour : MonoSingleton<BattleFieldBehaviour>
{
    private GameplayFsm gameplayFsm = new GameplayFsm();
    private GameplayContext gameplayContext = new GameplayContext();

    public GameplayFsm GameplayFsm { get => gameplayFsm; set => gameplayFsm = value; }
    public GameplayContext GameplayContext { get => gameplayContext; set => gameplayContext = value; }

    // Start is called before the first frame update
    void Start()
    {
        GameplayContext.Init();
        GameplayContext.CurStatusType = GameplayStatusType.GameplayStatus_WaitStart;

        GameplayFsm.Init(GameplayContext);

        GenerateMap();
    }

    // Generate the actual Hex-style map
    void GenerateMap()
    {
        GameObject Prefab = (GameObject)Resources.Load("Prefabs/BattlefieldTile");
        Prefab = Instantiate(Prefab);
        Prefab.transform.SetParent(this.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        GameplayFsm.Update(GameplayContext);
    }
}
