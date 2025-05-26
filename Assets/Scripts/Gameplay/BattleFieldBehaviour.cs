using System;
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

    public bool CanReach(int PosX, int PosY, int ReachPosX, int ReachPosY)
    {
        List<Tuple<int, int>> ReachTuple = Reach(PosX, PosY, 1);
        Debug.Log("PosX:" + PosX + " PosY:" + PosY + " ReachPosX:" + ReachPosX + " ReachPosY:" + ReachPosY + " ReachTuple:" + ReachTuple.ToShortString());
        foreach (Tuple<int, int> Pos in ReachTuple)
        {
            if (Pos.Item1 == ReachPosX && Pos.Item2 == ReachPosY)
            {
                return true;
            }
        }
        return false;
    }

    // maybe something wrong, but correct when radius = 1
    public List<Tuple<int, int>> Reach(int x, int y, int radius)
    {
        List<Tuple<int, int>> pos = new List<Tuple<int, int>>();
        int Llimit = x - radius;
        int Rlimit = x + radius;
        for (int i = Llimit; i <= Rlimit; i++)
        {
            if (i < 0 || i > battleFieldMaxWidth)
            {
                continue;
            }
            if (i == x)
            {
                continue;
            }
            Tuple<int, int> point = new Tuple<int, int>(i, y);
            pos.Add(point);
        }
        for (int i = 1; i <= radius; i++)
        {
            if (y % 2 == 1)
            {
                Llimit++;
            }
            else
            {
                Rlimit--;
            }
            for (int j = Llimit; j <= Rlimit; j++)
            {
                if (j < 0 || j > battleFieldMaxWidth)
                {
                    continue;
                }
                if (y + i < battleFieldMaxHeight)
                {
                    Tuple<int, int> upperPoint = new Tuple<int, int>(j, y + i);
                    pos.Add(upperPoint);
                }
                if (y - i >= 0)
                {
                    Tuple<int, int> lowerPoint = new Tuple<int, int>(j, y - i);
                    pos.Add(lowerPoint);
                }

            }
        }
        return pos;
    }

    private void RemoveTileMinion(int PosX, int PosY)
    {
        GameObject Tile = GetTile(PosX, PosY);
        Tile.GetComponent<BattleFieldTileBehaviour>().Minion = null;
        Tile.GetComponentInChildren<TMP_Text>().SetText(PosX + "-" + PosY);
    }
    
    public void MoveTile(int OldPosX, int OldPosY, int PosX, int PosY)
    {
        GameObject OldTile = GetTile(OldPosX, OldPosY);
        Minion OldMinion = OldTile.GetComponent<BattleFieldTileBehaviour>().Minion;
        OldMinion.RemainAction -= 1;

        GameObject NewTile = GetTile(PosX, PosY);
        NewTile.GetComponent<BattleFieldTileBehaviour>().Minion = new Minion();
        NewTile.GetComponent<BattleFieldTileBehaviour>().Minion.Copy(OldMinion);
        NewTile.GetComponentInChildren<TMP_Text>().SetText(OldMinion.Name);

        RemoveTileMinion(OldPosX, OldPosY);
    }

    public void AddTile(int PlayerChooseCardX, int PlayerChooseCardY, int PosX, int PosY)
    {
        if (PlayerChooseCardX == -1 || PlayerChooseCardY == -1)
        {
            return;
        }

        GameObject Tile = GetTile(PosX, PosY);

        Player CurPlayer = GameplayContext.Players[GameplayFsm.GetCurStatus(GameplayContext).CurPlayerIndex];
        // TODO ����Ժ��������ţ���Ҫע��
        Card ChooseCard = CurPlayer.CardList[PlayerChooseCardX];

        // TODO ����
        int CardId = ChooseCard.CardId;
        Minion CardMinion = new Minion();
        CardMinion.Attack = CardId;
        CardMinion.Defense = 1;
        CardMinion.Hitpoint = 1;
        CardMinion.Name = "lmh a"+ CardId+"-d1-h1";
        CardMinion.RemainAction = 3;
        CardMinion.MaxAction = 3;
        Tile.GetComponent<BattleFieldTileBehaviour>().Minion = CardMinion;

        Tile.GetComponentInChildren<TMP_Text>().SetText(CardMinion.Name);
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
