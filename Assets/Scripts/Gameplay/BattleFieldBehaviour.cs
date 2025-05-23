using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldBehaviour : MonoSingleton<BattleFieldBehaviour>
{
    private GameplayFsm gameplayFsm = new GameplayFsm();
    private GameplayContext gameplayContext = new GameplayContext();
    private Dictionary<int, Dictionary<int, GameObject>> buttonMap = new Dictionary<int, Dictionary<int, GameObject>>();
    public GameplayFsm GameplayFsm { get => gameplayFsm; set => gameplayFsm = value; }
    public GameplayContext GameplayContext { get => gameplayContext; set => gameplayContext = value; }
    public Dictionary<int, Dictionary<int, GameObject>> ButtonMap { get => buttonMap; set => buttonMap = value; }

    private const float edgeLength = 60.0f;
    private const int battleFieldMaxWidth = 8;
    private const int battleFieldMaxHeight = 3;

    // Start is called before the first frame update
    void Start()
    {
        GameplayContext.Init();
        GameplayContext.CurStatusType = GameplayStatusType.GameplayStatus_WaitStart;

        GameplayFsm.Init(GameplayContext);
        GameplayContext.GpFsm = GameplayFsm;

        GenerateBattleFieldTiles();
    }

    private GameObject InstantiateTile(int PosX, int PosY)
    {
        float CanvasY = -593.0f;
        GameObject Prefab = Instantiate((GameObject)Resources.Load("Prefabs/BattleFieldTile"));
        Prefab.transform.SetParent(this.transform, false);
        float StartX = 200f;
        float StartY = 0f;
        float RealX = 1.732f * PosX * edgeLength + (PosY % 2) * 1.732f / 2 * edgeLength;
        float RealY = -270 - CanvasY + 1.5f * PosY * edgeLength;
        Prefab.transform.position = new Vector3(StartX + RealX, StartY + RealY, 0);
        TMP_Text PrefabText = Prefab.GetComponentInChildren<TMP_Text>();
        PrefabText.text = PosX + "-" + PosY;
        Prefab.name = "BattleFieldTile-" + PosX + "-" + PosY;

        BattleFieldTileBehaviour PrefabBehaviour = Prefab.GetComponent<BattleFieldTileBehaviour>();
        PrefabBehaviour.Name = Prefab.name;
        PrefabBehaviour.PosX = PosX;
        PrefabBehaviour.PosY = PosY;
        PrefabBehaviour.CoordX = StartX + RealX;
        PrefabBehaviour.CoordY = StartY + RealY + CanvasY;

        Button PrefabButton = Prefab.GetComponentInChildren<Button>();
        PrefabButton.onClick.AddListener(() => TileOnClick(PosX, PosY));

        return Prefab;
    }

    // Generate the actual Hex-style map
    private void GenerateBattleFieldTiles()
    {
        for (int i = 0; i < battleFieldMaxWidth; i++)
        {
            for (int j = 0; j < battleFieldMaxHeight; j++)
            {
                GameObject Prefab = InstantiateTile(i, j);
                if (ButtonMap.ContainsKey(i) == false)
                {
                    Dictionary<int, GameObject> Pairs = new Dictionary<int, GameObject>();
                    Pairs.Add(j, Prefab);
                    ButtonMap.Add(i, Pairs);
                }
                else
                {
                    ButtonMap[i].Add(j, Prefab);
                }
            }
        }
    }

    public void ShowTile(int PlayerChooseCardX, int PlayerChooseCardY, int PosX, int PosY)
    {
        if (PlayerChooseCardX == -1 || PlayerChooseCardY == -1)
        {
            return;
        }

        GameObject Tile = GetTile(PosX, PosY);

        Player CurPlayer = GameplayContext.Players[GameplayFsm.GetCurStatus(GameplayContext).CurPlayerIndex];
        // TODO 如果以后变成了两排，需要注意
        Card ChooseCard = CurPlayer.CardList[PlayerChooseCardX];
        Tile.GetComponentInChildren<TMP_Text>().SetText(ChooseCard.Name);
    }

    public GameObject GetTile(int PosX, int PosY)
    {
        if (ButtonMap.ContainsKey(PosX) == false)
        {
            Debug.LogError("tile not find" + PosX + " " + PosY);
            return null;
        }
        if (ButtonMap[PosX].ContainsKey(PosY) == false)
        {
            Debug.LogError("tile not find" + PosX + " " + PosY);
            return null;
        }
        return ButtonMap[PosX][PosY];
    }

    public void TileOnClick(int PosX, int PosY)
    {
        GameplayEvent GpEvent = new GameplayEvent();
        GpEvent.Type = GameplayEventType.GameplayEventType_ClickTile;
        GpEvent.ClickTileEvent = new GameplayEventClickTile();
        GpEvent.ClickTileEvent.PosX = PosX;
        GpEvent.ClickTileEvent.PosY = PosY;
        GameplayFsm.ProcessEvent(GameplayContext, GpEvent);
    }

    // Update is called once per frame
    void Update()
    {
        GameplayFsm.Update(GameplayContext);
    }
}
