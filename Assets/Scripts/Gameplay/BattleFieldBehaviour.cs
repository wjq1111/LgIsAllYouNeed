using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class BattleFieldBehaviour : MonoSingleton<BattleFieldBehaviour>
{
    private GameplayFsm gameplayFsm = new GameplayFsm();
    private GameplayContext gameplayContext = new GameplayContext();
    private Dictionary<int, Dictionary<int, GameObject>> tileMap = new Dictionary<int, Dictionary<int, GameObject>>();

    private GameObject dummy;
    public GameplayFsm GameplayFsm { get => gameplayFsm; set => gameplayFsm = value; }
    public GameplayContext GameplayContext { get => gameplayContext; set => gameplayContext = value; }
    public Dictionary<int, Dictionary<int, GameObject>> TileMap { get => tileMap; set => tileMap = value; }
    public GameObject Dummy { get => dummy; set => dummy = value; }


    private const float edgeLength = 60.0f;
    private const int battleFieldMaxWidth = 8;
    private const int battleFieldMaxHeight = 5;


    // Start is called before the first frame update
    void Start()
    {
        GameplayContext.Init();
        GameplayContext.CurStatusType = GameplayStatusType.GameplayStatus_WaitStart;

        GameplayFsm.Init(GameplayContext);
        GameplayContext.GpFsm = GameplayFsm;
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
    public void GenerateBattleFieldTiles()
    {
        for (int i = 0; i < battleFieldMaxWidth; i++)
        {
            for (int j = 0; j < battleFieldMaxHeight; j++)
            {
                GameObject Prefab = InstantiateTile(i, j);
                if (TileMap.ContainsKey(i) == false)
                {
                    Dictionary<int, GameObject> Pairs = new Dictionary<int, GameObject>();
                    Pairs.Add(j, Prefab);
                    TileMap.Add(i, Pairs);
                }
                else
                {
                    TileMap[i].Add(j, Prefab);
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
            if ((y - i) % 2 == 0)
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
        OldMinion.RemainMovement -= 1;

        GameObject NewTile = GetTile(PosX, PosY);
        NewTile.GetComponent<BattleFieldTileBehaviour>().Minion = new Minion();
        NewTile.GetComponent<BattleFieldTileBehaviour>().Minion.Copy(OldMinion);
        NewTile.GetComponentInChildren<TMP_Text>().SetText(OldMinion.Name);

        GameFramework.Instance.BattleLog(OldMinion.Name + " moves from ("+ OldPosX + ", " + OldPosY + ") To (" + PosX + ", " + PosY + ")");

        RemoveTileMinion(OldPosX, OldPosY);
    }

    public void Combat(int atkrPosX, int atkrPosY, int dfsrPosX, int dfsrPosY)
    {
        GameObject atkrTile = GetTile(atkrPosX, atkrPosY);
        Minion atkr = atkrTile.GetComponent<BattleFieldTileBehaviour>().Minion;

        GameObject dfsrTile = GetTile(dfsrPosX, dfsrPosY);
        Minion dfsr = dfsrTile.GetComponent<BattleFieldTileBehaviour>().Minion;

        atkr.RemainAction -= 1;

        do
        {
            // 攻击
            if (atkr.Attack >= dfsr.Defense)
            {
                if (atkr.Attack == dfsr.Defense)
                {
                    GameFramework.Instance.BattleLog(dfsr.Name + " lose " + (atkr.Attack - dfsr.Defense).ToString() + " hitpoint(s)!");
                    break;
                }
                dfsr.Hitpoint -= atkr.Attack - dfsr.Defense;
                GameFramework.Instance.BattleLog(dfsr.Name + " lose " + (atkr.Attack - dfsr.Defense).ToString() + " hitpoint(s)!");
                if (dfsr.Hitpoint < 0)
                {
                    RemoveTileMinion(dfsrPosX, dfsrPosY);
                    GameFramework.Instance.BattleLog(dfsr.Name + " dead!");
                    break;
                }
            }
            // 反击
            if (dfsr.Attack >= atkr.Defense)
            {
                if (dfsr.Attack == atkr.Defense)
                {
                    GameFramework.Instance.BattleLog(atkr.Name + " lose " + (dfsr.Attack - atkr.Defense).ToString() + " hitpoint(s)!");
                    break;
                }
                atkr.Hitpoint -= dfsr.Attack - atkr.Defense;
                GameFramework.Instance.BattleLog(atkr.Name + " lose " + (dfsr.Attack - atkr.Defense).ToString() + " hitpoint(s)!");
                if (atkr.Hitpoint < 0)
                {
                    // atkr.die(int killer)
                    RemoveTileMinion(atkrPosX, atkrPosY);
                    GameFramework.Instance.BattleLog(atkr.Name + " dead!");
                    break;
                }
            }
        } while (false);
    }

    public bool NeedFinishGame()
    {
        if (Dummy == null)
        {
            return true;
        }
        if (Dummy.GetComponent<BattleFieldTileBehaviour>() == null)
        {
            return true;
        }
        if (Dummy.GetComponent<BattleFieldTileBehaviour>().Minion == null)
        {
            return true;
        }
        if (Dummy.GetComponent<BattleFieldTileBehaviour>().Minion.Hitpoint <= 0)
        {
            return true;
        }
        // 自己死了

        return false;
    }

    public void AddEnemy()
    {
        // TODO 现在只有一关
        int CurStageId = 1;
        StageConfigItem StageConfigItem = ConfigMgr.Instance.StageConfig.StageConfigMap[CurStageId];
        MinionConfigItem MinionConfigItem = ConfigMgr.Instance.MinionConfig.MinionConfigMap[StageConfigItem.BossMinionId];

        Minion CardMinion = new Minion();
        CardMinion.Attack = MinionConfigItem.Attack;
        CardMinion.Defense = MinionConfigItem.Defense;
        CardMinion.Hitpoint = MinionConfigItem.HitPoint;
        CardMinion.Name = MinionConfigItem.Name;
        CardMinion.RemainAction = MinionConfigItem.BaseAction;
        CardMinion.MaxAction = MinionConfigItem.BaseAction;
        CardMinion.RemainMovement = MinionConfigItem.BaseMovement;
        CardMinion.MaxMovement = MinionConfigItem.BaseMovement;
        CardMinion.PlayerId = (int)EPlayerIndex.EnemyPlayerIndex;
        Dummy = GetTile(StageConfigItem.TilePosX, StageConfigItem.TilePosY);
        Dummy.GetComponent<BattleFieldTileBehaviour>().Minion = CardMinion;
        Dummy.GetComponentInChildren<TMP_Text>().SetText(CardMinion.Name);

        GameFramework.Instance.BattleLog(CardMinion.Name + " arrived!");
        foreach (EnemyMinion enemy in StageConfigItem.EnemyMinions)
        {
            Minion minion = new Minion();
            minion.InitByMinionConfig(enemy.MinionId, (int)EPlayerIndex.EnemyPlayerIndex);
            GameObject enemyTile = GetTile(enemy.TilePosX, enemy.TilePosY);
            enemyTile.GetComponent<BattleFieldTileBehaviour>().Minion = minion;
            enemyTile.GetComponentInChildren<TMP_Text>().SetText(minion.Name);

            GameFramework.Instance.BattleLog(minion.Name + " arrived!");
        }

    }
    public void AddTile(int PlayerChooseCardX, int PlayerChooseCardY, int PosX, int PosY)
    {
        if (PlayerChooseCardX == -1 || PlayerChooseCardY == -1)
        {
            return;
        }

        GameObject Tile = GetTile(PosX, PosY);

        Player CurPlayer = GameplayContext.Players[GameplayFsm.CurPlayerIndex];
        // TODO 如果以后变成了两排，需要注意
        Card ChooseCard = CurPlayer.CardList[PlayerChooseCardX];
        // 这张卡的召唤属性已经被记录到卡上了，所以Minion在生成的时候需要用Card的生成，不能再读配置
        // 如果有一些永久加成的属性加到卡上（本局游戏/本次存档），直接改玩家的手牌
        foreach (BaseEffect CardEffect in ChooseCard.CardEffect)
        {
            if (CardEffect.EffectType == EffectType.EffectType_Summon)
            {
                SummonEffect SummonEffect = (SummonEffect)CardEffect;
                Minion CardMinion = new Minion();
                CardMinion.InitBySummonEffect(SummonEffect, ChooseCard.PlayerId);
                Tile.GetComponent<BattleFieldTileBehaviour>().Minion = CardMinion;
                Tile.GetComponentInChildren<TMP_Text>().SetText(CardMinion.Name);

                GameFramework.Instance.BattleLog(CardMinion.Name + " arrived!");
            }
            else
            {
                // TODO
            }
        }
    }

    public GameObject GetTile(int PosX, int PosY)
    {
        if (TileMap.ContainsKey(PosX) == false)
        {
            Debug.LogError("tile not find" + PosX + " " + PosY);
            return null;
        }
        if (TileMap[PosX].ContainsKey(PosY) == false)
        {
            Debug.LogError("tile not find" + PosX + " " + PosY);
            return null;
        }
        return TileMap[PosX][PosY];
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
