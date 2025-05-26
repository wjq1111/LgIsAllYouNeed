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
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 100;
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

        CardDeck.Instance.Init(Handbook.Instance.UnlockedCards);

        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.StartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 2;

        // 系统发牌，直接加到player手牌
        List<Card> CardList = CardDeck.Instance.DrawCards(GameFramework.Instance.CardNumPerRound);
        if (GpCurStatus.CurPlayerIndex > Context.Players.Count)
        {
            Debug.LogError("cur player index invalid " + GpCurStatus.CurPlayerIndex);
            return -1;
        }
        Context.Players[GpCurStatus.CurPlayerIndex].AddCard(CardList);

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
            // 把牌渲染到CardSession上
            int PlayerIndex = GetCurStatus(Context).CurPlayerIndex;
            Player CurPlayer = Context.Players[PlayerIndex];

            GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
            CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
            CardSessionBehaviour.ShowCards(CurPlayer.CardList);

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
        return base.OnEnter(Context);
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);

        if (GpEvent.Type == GameplayEventType.GameplayEventType_ClickCard)
        {
            GameplayStatus GpCurStatus = GetCurStatus(Context);
            Player CurPlayer = Context.Players[GpCurStatus.CurPlayerIndex];

            if (CurPlayer.CardList[GpEvent.ClickCardEvent.PosX].CardId != 0)
            {
                CurPlayer.ChooseCardPosX = GpEvent.ClickCardEvent.PosX;
                CurPlayer.ChooseCardPosY = GpEvent.ClickCardEvent.PosY;
            }
        }

        if (GpEvent.Type == GameplayEventType.GameplayEventType_ClickTile)
        {
            GameplayStatus GpCurStatus = GetCurStatus(Context);
            Player CurPlayer = Context.Players[GpCurStatus.CurPlayerIndex];

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
                        CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                        CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                    }
                    else if (BattleFieldBehaviour.CanReach(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY))
                    {
                        if (OldMinion.RemainAction > 0)
                        {
                            // 先去掉原位置的棋子，再加上新位置的棋子
                            BattleFieldBehaviour.MoveTile(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY);
                            BattleFieldBehaviour.GetTile(GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY);
                            CurPlayer.ResetChooseBattleFieldTile();
                        }
                    }
                    else
                    {
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

                // 把玩家选的那个卡 变成棋子 放到玩家选的那个格子上
                GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                BattleFieldBehaviour.AddTile(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY, TilePosX, TilePosY);

                // 删除玩家手牌，马上重新渲染card session
                CurPlayer.DelCard(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY);
                GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
                CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
                CardSessionBehaviour.ShowCards(CurPlayer.CardList);

                CurPlayer.ResetChooseCard();
            }
        }
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        return base.OnUpdate(Context);
    }
}

public class RoundEndAction : GameplayAction
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
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.CurPlayerIndex = (GpCurStatus.CurPlayerIndex + 1) % Context.Players.Count;
        Debug.Log("cur player index -> " + GpCurStatus.CurPlayerIndex);
        return 0;
    }
}



public class GameplayActionMgr : Singleton<GameplayActionMgr>
{
    private WaitStartAction waitStartAction = new WaitStartAction();
    private RoundStartAction roundStartAction = new RoundStartAction();
    private RoundingAction roundingAction = new RoundingAction();
    private RoundEndAction roundEndAction = new RoundEndAction();

    private Dictionary<GameplayStatusType, GameplayAction> actionMap = new Dictionary<GameplayStatusType, GameplayAction>();

    public new void Init()
    {
        actionMap.Add(GameplayStatusType.GameplayStatus_WaitStart, waitStartAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_RoundStart, roundStartAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_Rounding, roundingAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_RoundEnd, roundEndAction);
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