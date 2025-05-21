using System;
using System.Collections;
using System.Collections.Generic;
using static System.DateTimeOffset;
using UnityEngine;

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
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 10;
        return 0;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // ³¬Ê±
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
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.StartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 10;
        return 0;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // ³¬Ê±
            Debug.LogError("round start timeout");
            return -1;
        }
        // update
        return 0;
    }
}

public class GameplayActionMgr : Singleton<GameplayActionMgr>
{
    private WaitStartAction waitStartAction = new WaitStartAction();
    private RoundStartAction roundStartAction = new RoundStartAction();
    private GameplayAction roundingAction = new GameplayAction();
    private GameplayAction roundEndAction = new GameplayAction();

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