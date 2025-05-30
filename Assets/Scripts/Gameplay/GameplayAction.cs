using System;
using System.Collections;
using System.Collections.Generic;
using static System.DateTimeOffset;
using UnityEngine;
using UnityEditor.MPE;
using Unity.Mathematics;

public class GameplayAction
{
    public virtual int OnEnter(GameplayContext Context)
    {
        Debug.Log("enter" + Context.CurStatusType); 
        return 0;
    }
    public virtual int OnUpdate(GameplayContext Context)
    {
        return 0;
    }
    public virtual int OnError(GameplayContext Context)
    {
        Debug.Log("error" + Context.CurStatusType);
        return 0;
    }
    public virtual GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        Debug.Log("event" + Context.CurStatusType + " " + GpEvent.Type);
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }
    public virtual int OnExit(GameplayContext Context, GameplayEvent GpEvent)
    {
        Debug.Log("exit" + Context.CurStatusType);
        return 0;
    }
    public GameplayStatus GetCurStatus(GameplayContext Context) 
    {
        return Context.GetStatus(Context.CurStatusType);
    }
}

public class WaitStartAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        base.OnEnter(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.StartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 10000;

        CardDeck.Instance.Init(Handbook.Instance.UnlockedCards, Context.GpFsm.CurPlayerIndex);
        return 0;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // 超时
            Debug.LogError("wait start timeout");
            return -1;
        }
        // update
        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);
        if (GpEvent.Type == GameplayEventType.GameplayEventType_StartGame)
        {
            GameFramework.Instance.BattleLog("Game Start!");

            GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
            BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
            BattleFieldBehaviour.GenerateBattleFieldTiles();
            BattleFieldBehaviour.InitPlayerObject();

            GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
            CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
            CardSessionBehaviour.GenerateCardSession();
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }
}

public class RoundStartAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        base.OnEnter(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.StartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 2;
        if (Context.GpFsm.CurPlayerIndex > Context.Players.Count)
        {
            Debug.LogError("cur player index invalid " + Context.GpFsm.CurPlayerIndex);
            return -1;
        }

        // 系统发牌，直接加到player手牌
        GameObject CardSession = GameFramework.DfsObj(GameFramework.Instance.StartPrefab.transform, "CardSession").gameObject;
        CardSessionBehaviour CardSessionBehaviour = CardSession.GetComponent<CardSessionBehaviour>();

        int CurRoundDrawCardNum = GameFramework.Instance.CardNumPerRound;
        if (Context.Players[Context.GpFsm.CurPlayerIndex].CardList.Count + CurRoundDrawCardNum > CardSessionBehaviour.CardMaxCount)
        {
            CurRoundDrawCardNum = CardSessionBehaviour.CardMaxCount - Context.Players[Context.GpFsm.CurPlayerIndex].CardList.Count;
        }
        List<Card> CardList = CardDeck.Instance.DrawCards(CurRoundDrawCardNum);

        Context.Players[Context.GpFsm.CurPlayerIndex].AddCard(CardList);

        GameObject BattleField = GameFramework.Instance.GetBattleFieldObj();
        BattleFieldBehaviour BattleFieldBehaviour = BattleField.GetComponent<BattleFieldBehaviour>();
        // 己方Minion回合reset操作
        Dictionary<int, Dictionary<int, GameObject>> TileMap = GameFramework.Instance.GetBattleFieldObj().GetComponent<BattleFieldBehaviour>().TileMap;
        Dictionary<int, Dictionary<int, bool>> NeedRemoveMinion = new Dictionary<int, Dictionary<int, bool>>();
        foreach (var Dict in TileMap)
        {
            foreach (var Tile in Dict.Value)
            {
                Minion Minion = Tile.Value.GetComponent<BattleFieldTileBehaviour>().Minion;
                if (Minion != null && Minion.PlayerId == Context.GpFsm.CurPlayerIndex)
                {
                    Minion.RoundStartReset();
                    // 检查召唤物是不是死了
                    if (Minion.Hitpoint <= 0)
                    {
                        if (NeedRemoveMinion.ContainsKey(Dict.Key) == false)
                        {
                            Dictionary<int, bool> keyValuePairs = new Dictionary<int, bool>();
                            keyValuePairs[Tile.Key] = true;
                            NeedRemoveMinion[Dict.Key] = keyValuePairs;
                        }
                        else
                        {
                            NeedRemoveMinion[Dict.Key].Add(Tile.Key, true);
                        }
                    }
                }
            }
        }
        // 清理掉死的召唤物
        foreach (var PosX in NeedRemoveMinion.Keys)
        {
            foreach (var PosY in NeedRemoveMinion[PosX].Keys)
            {
                GameObject Tile = BattleFieldBehaviour.GetTile(PosX, PosY);
                GameFramework.Instance.BattleLog(Tile.GetComponent<BattleFieldTileBehaviour>().Minion.Name + " dead!");
                BattleFieldBehaviour.RemoveTileMinion(PosX, PosY);
            }
        }

        GameplayEvent GpEvent = new GameplayEvent();
        GpEvent.Type = GameplayEventType.GameplayEventType_DrawCard;
        Context.GpFsm.ProcessEvent(Context, GpEvent);

        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);
        if (GpEvent.Type == GameplayEventType.GameplayEventType_DrawCard)
        {
            int PlayerIndex = Context.GpFsm.CurPlayerIndex;
            if (PlayerIndex == (int)EPlayerIndex.RealPlayerIndex)
            {
                // 把牌渲染到CardSession上
                Player CurPlayer = Context.Players[PlayerIndex];

                GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
                CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
                CardSessionBehaviour.ShowCards(CurPlayer.CardList);
            }
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }

        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // 超时
            return -1;
        }
        // update
        return 0;
    }
}

public class RoundingAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        base.OnEnter(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.StartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 60;
        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);

        if (GpEvent.Type == GameplayEventType.GameplayEventType_EndRound)
        {
            GameFramework.Instance.BattleLog("cur player " + Context.GpFsm.CurPlayerIndex + " round end!");
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }

        if (GpEvent.Type == GameplayEventType.GameplayEventType_CheckFinishGame)
        {
            GameplayFinishReason Reason = GameFramework.Instance.GetBattleFieldObj().GetComponent<BattleFieldBehaviour>().NeedFinishGame();
            if (Reason != GameplayFinishReason.GameplayFinishReason_Invalid)
            {
                GameFramework.Instance.BattleLog("Game End!");
                return GameplayEventResult.GameplayEventResult_NeedSwitch;
            }
            return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
        }

        // 只有玩家回合点击才有效果
        if (Context.GpFsm.CurPlayerIndex == (int)EPlayerIndex.RealPlayerIndex)
        {
            if (GpEvent.Type == GameplayEventType.GameplayEventType_ClickCard)
            {
                GameplayStatus GpCurStatus = GetCurStatus(Context);
                Player CurPlayer = Context.Players[Context.GpFsm.CurPlayerIndex];

                if (CurPlayer.CardList[GpEvent.ClickCardEvent.PosX].CardId != 0)
                {
                    CurPlayer.ChooseCardPosX = GpEvent.ClickCardEvent.PosX;
                    CurPlayer.ChooseCardPosY = GpEvent.ClickCardEvent.PosY;
                }
            }

            if (GpEvent.Type == GameplayEventType.GameplayEventType_ClickTile)
            {
                GameplayStatus GpCurStatus = GetCurStatus(Context);
                Player CurPlayer = Context.Players[Context.GpFsm.CurPlayerIndex];

                if (CurPlayer.ChooseCardPosX == -1 && CurPlayer.ChooseCardPosY == -1)
                {
                    // 玩家已选择的卡的位置是非法值，说明没有点手牌，直接点了tile
                    // 判断tile上有没有棋子，如果有的话，在下一次点tile的时候，移动棋子
                    if (CurPlayer.ChooseBattleFileTilePosX == -1 && CurPlayer.ChooseBattleFileTilePosY == -1)
                    {
                        CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                        CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                    }
                    else
                    {
                        GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                        BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                        // 如果上一次选择的格子是空的，则重置选择的格子，如果相同，视为没操作
                        GameObject OldChooseTile = BattleFieldBehaviour.GetTile(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY);
                        Minion OldMinion = OldChooseTile.GetComponent<BattleFieldTileBehaviour>().Minion;
                        if (OldMinion == null)
                        {
                            // 空选，当做没选
                            CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                            CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                        }
                        else if (BattleFieldBehaviour.CanReach(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY))
                        {
                            // 两位置之间的距离合法，且新位置没有怪物
                            Minion NewMinion = BattleFieldBehaviour.GetTile(GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY).GetComponent<BattleFieldTileBehaviour>().Minion;
                            if (NewMinion == null)
                            {
                                if (OldMinion.RemainMovement > 0)
                                {
                                    // 先去掉原位置的棋子，再加上新位置的棋子
                                    BattleFieldBehaviour.MoveTile(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY);
                                }
                                else
                                {
                                    // 行动力不足，暂时打个日志，理想状态应该是提示一下
                                    Debug.Log(OldMinion.Name + " cannot move ");
                                }
                                CurPlayer.ResetChooseBattleFieldTile();
                            }
                            // 新位置是己方，不可移动
                            else if (NewMinion.PlayerId == OldMinion.PlayerId)
                            {
                                Debug.Log(OldMinion.Name + " is blocked by " + NewMinion.Name);
                                CurPlayer.ResetChooseBattleFieldTile();
                            }
                            // 新位置是敌方，战斗
                            else
                            {
                                if (OldMinion.RemainAction > 0)
                                {
                                    BattleFieldBehaviour.Combat(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY);
                                    Debug.Log(OldMinion.Name + " fight with " + NewMinion.Name);
                                }
                                CurPlayer.ResetChooseBattleFieldTile();
                            }
                        }
                        else
                        {
                            // 两者之间的距离不合法，无法移动
                            CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                            CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                        }
                    }

                    return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
                }
                else
                {
                    // 玩家已选择的卡的位置不是非法值，认为玩家已经选了卡，现在要放到某个格子上
                    int TilePosX = GpEvent.ClickTileEvent.PosX;
                    int TilePosY = GpEvent.ClickTileEvent.PosY;

                    // 把玩家选的那个卡 施加到格子上的效果
                    GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                    BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                    BattleFieldBehaviour.UseCard(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY, TilePosX, TilePosY);

                    // 删除玩家手牌，马上重新渲染card session
                    CurPlayer.DelCard(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY);
                    GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
                    CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
                    CardSessionBehaviour.ShowCards(CurPlayer.CardList);

                    CurPlayer.ResetChooseCard();
                }
            }
        }
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // 超时
            Context.GpFsm.ProcessEvent(Context, GameplayEventType.GameplayEventType_EndRound);
            return 0;
        }
        // update
        return 0;
    }
}

public class RoundEndAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        base.OnEnter(Context);
        Context.GpFsm.ProcessEvent(Context, GameplayEventType.GameplayEventType_NewRound);
        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);
        if (GpEvent.Type == GameplayEventType.GameplayEventType_NewRound)
        {
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        return base.OnUpdate(Context);
    }

    public override int OnExit(GameplayContext Context, GameplayEvent GpEvent)
    {
        Context.GpFsm.CurPlayerIndex = (Context.GpFsm.CurPlayerIndex + 1) % Context.Players.Count;
        Debug.Log("cur player index -> " + Context.GpFsm.CurPlayerIndex);
        return 0;
    }
}

public class FinishGameAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        return base.OnEnter(Context);
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        return base.OnEvent(Context, GpEvent);
    }

    public override int OnUpdate(GameplayContext Context)
    {
        return base.OnUpdate(Context);
    }

    public override int OnExit(GameplayContext Context, GameplayEvent GpEvent)
    {
        return base.OnExit(Context, GpEvent);
    }
}



public class GameplayActionMgr : Singleton<GameplayActionMgr>
{
    private WaitStartAction waitStartAction = new WaitStartAction();
    private RoundStartAction roundStartAction = new RoundStartAction();
    private RoundingAction roundingAction = new RoundingAction();
    private RoundEndAction roundEndAction = new RoundEndAction();
    private FinishGameAction finishGameAction = new FinishGameAction();

    private Dictionary<GameplayStatusType, GameplayAction> actionMap = new Dictionary<GameplayStatusType, GameplayAction>();

    public new void Init()
    {
        actionMap.Add(GameplayStatusType.GameplayStatus_WaitStart, waitStartAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_RoundStart, roundStartAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_Rounding, roundingAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_RoundEnd, roundEndAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_FinishGame, finishGameAction);
    }

    public GameplayAction GetAction(GameplayStatusType GpStatus)
    {
        if (actionMap == null)
        {
            Debug.LogError("action map null");
            return null;
        }
        if (actionMap.ContainsKey(GpStatus) == false)
        {
            Debug.LogError("action map not contains status: " + GpStatus);
            return null;
        }
        return actionMap[GpStatus];
    }
}