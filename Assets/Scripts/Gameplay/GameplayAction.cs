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
            // ��ʱ
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

        // ϵͳ���ƣ�ֱ�Ӽӵ�player����
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
            // ������Ⱦ��CardSession��
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
            // ��ʱ
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
                // �����ѡ��Ŀ���λ���ǷǷ�ֵ��˵��û�е����ƣ�ֱ�ӵ���tile
                // �ж�tile����û�����ӣ�����еĻ�������һ�ε�tile��ʱ���ƶ�����
                if (CurPlayer.ChooseBattleFileTilePosX == -1 && CurPlayer.ChooseBattleFileTilePosY == -1)
                {
                    CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                    CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                }
                else
                {
                    GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                    BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                    // �����һ��ѡ��ĸ����ǿյģ�������ѡ��ĸ��ӣ������ͬ����Ϊû����
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
                            // ��ȥ��ԭλ�õ����ӣ��ټ�����λ�õ�����
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
                // �����ѡ��Ŀ���λ�ò��ǷǷ�ֵ����Ϊ����Ѿ�ѡ�˿�������Ҫ�ŵ�ĳ��������
                int TilePosX = GpEvent.ClickTileEvent.PosX;
                int TilePosY = GpEvent.ClickTileEvent.PosY;

                // �����ѡ���Ǹ��� ������� �ŵ����ѡ���Ǹ�������
                GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                BattleFieldBehaviour.AddTile(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY, TilePosX, TilePosY);

                // ɾ��������ƣ�����������Ⱦcard session
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