using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayFsm
{
    // 状态转换图
    private Dictionary<GameplayStatusType, Dictionary<GameplayEventType, GameplayStatusType>> transitionMap = new Dictionary<GameplayStatusType, Dictionary<GameplayEventType, GameplayStatusType>>();
    // action列表
    private Dictionary<GameplayStatusType, GameplayAction> actionMap = new Dictionary<GameplayStatusType, GameplayAction>();

    // 事件队列，必须前一个事件处理完再处理下一个事件
    public Queue<GameplayEvent> eventQueue = new Queue<GameplayEvent>();

    public Dictionary<GameplayStatusType, Dictionary<GameplayEventType, GameplayStatusType>> TransitionMap { get => transitionMap; set => transitionMap = value; }
    public Dictionary<GameplayStatusType, GameplayAction> ActionMap { get => actionMap; set => actionMap = value; }


    private int curPlayerIndex = 0;
    public int CurPlayerIndex { get => curPlayerIndex; set => curPlayerIndex = value; }


    // 初始化函数
    public void Init(GameplayContext Context)
    {
        // 注册状态转换图
        RegisterTransition(GameplayStatusType.GameplayStatus_WaitStart, GameplayEventType.GameplayEventType_StartGame, GameplayStatusType.GameplayStatus_RoundStart);

        RegisterTransition(GameplayStatusType.GameplayStatus_RoundStart, GameplayEventType.GameplayEventType_DrawCard, GameplayStatusType.GameplayStatus_Rounding);

        RegisterTransition(GameplayStatusType.GameplayStatus_Rounding, GameplayEventType.GameplayEventType_EndRound, GameplayStatusType.GameplayStatus_RoundEnd);
        RegisterTransition(GameplayStatusType.GameplayStatus_Rounding, GameplayEventType.GameplayEventType_CheckFinishGame, GameplayStatusType.GameplayStatus_FinishGame);

        RegisterTransition(GameplayStatusType.GameplayStatus_RoundEnd, GameplayEventType.GameplayEventType_NewRound, GameplayStatusType.GameplayStatus_RoundStart);


        // 注册action列表
        RegisterAction(GameplayStatusType.GameplayStatus_WaitStart);
        RegisterAction(GameplayStatusType.GameplayStatus_RoundStart);
        RegisterAction(GameplayStatusType.GameplayStatus_Rounding);
        RegisterAction(GameplayStatusType.GameplayStatus_RoundEnd);
        RegisterAction(GameplayStatusType.GameplayStatus_FinishGame);


        // 处理第一个状态
        GameplayAction GpCurAction = GetAction(Context, Context.CurStatusType);
        GpCurAction.OnEnter(Context);
    }

    private void RegisterTransition(GameplayStatusType GpStatusType, GameplayEventType GpEventType, GameplayStatusType GpNewStatus)
    {
        if (transitionMap.ContainsKey(GpStatusType) == false)
        {
            Dictionary<GameplayEventType, GameplayStatusType> Pairs = new Dictionary<GameplayEventType, GameplayStatusType>();
            Pairs.Add(GpEventType, GpNewStatus);
            transitionMap.Add(GpStatusType, Pairs);
        }
        if (transitionMap[GpStatusType].ContainsKey(GpEventType) == false)
        {
            TransitionMap[GpStatusType].Add(GpEventType, GpNewStatus);
        }
    }

    private void RegisterAction(GameplayStatusType GpStatusType)
    {
        actionMap.Add(GpStatusType, GameplayActionMgr.Instance.GetAction(GpStatusType));
    }

    // 获取当前状态
    public GameplayStatus GetCurStatus(GameplayContext Context)
    {
        return Context.GetStatus(Context.CurStatusType);
    }

    // 获取action
    public GameplayAction GetAction(GameplayContext Context, GameplayStatusType GpStatusType)
    {
        if (actionMap.ContainsKey(GpStatusType) == false)
        {
            Debug.LogError("action map not contains status:" + GpStatusType);
            return null;
        }
        return actionMap[GpStatusType];
    }

    // 获取下一个状态
    public GameplayStatusType GetNextStatus(GameplayContext Context, GameplayStatusType GpStatusType, GameplayEventType GpEventType)
    {
        if (transitionMap.ContainsKey(GpStatusType) == false)
        {
            Debug.LogError("transition map not contains transition:" + GpStatusType + " " + GpEventType);
            return GameplayStatusType.GameplayStatus_Invalid;
        }
        if (transitionMap[GpStatusType].ContainsKey(GpEventType) == false)
        {
            Debug.LogError("transition map not contains transition:" + GpStatusType + " " + GpEventType);
            return GameplayStatusType.GameplayStatus_Invalid;
        }
        return transitionMap[GpStatusType][GpEventType];
    }

    // update
    public int Update(GameplayContext Context)
    {
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.GpType == GameplayStatusType.GameplayStatus_Invalid)
        {
            Debug.LogError("wrong type");
            return -1;
        }
        GameplayAction GpCurAction = GetAction(Context, GpCurStatus.GpType);
        return GpCurAction.OnUpdate(Context);
    }

    public int ProcessEvent(GameplayContext Context, GameplayEventType GpEventType)
    {
        GameplayEvent GpNewEvent = new GameplayEvent();
        GpNewEvent.Type = GpEventType;
        return ProcessEvent(Context, GpNewEvent);
    }

    // 处理事件
    public int ProcessEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        if (eventQueue.Count > 20)
        {
            Debug.LogError("event queue full, maybe something wrong");
            return -1;
        }
        eventQueue.Enqueue(GpEvent);
        if (eventQueue.Count > 1)
        {
            return 0;
        }
        while (eventQueue.Count > 0)
        {
            GameplayEvent CurEvent = eventQueue.First();
            int Ret = ExecuteEvent(Context, CurEvent);
            if (Ret != 0)
            {
                GameplayStatus GpCurStatus = GetCurStatus(Context);
                GameplayAction GpCurAction = GetAction(Context, GpCurStatus.GpType);
                return GpCurAction.OnError(Context);
            }
            eventQueue.Dequeue();
        }
        return 0;
    }

    // 执行事件
    public int ExecuteEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GameplayAction GpCurAction = GetAction(Context, GpCurStatus.GpType);

        GameplayEventResult GpEventResult = GpCurAction.OnEvent(Context, GpEvent);
        if (GpEventResult == GameplayEventResult.GameplayEventResult_NoNeedSwitch)
        {
            return 0;
        }

        int Ret = GpCurAction.OnExit(Context, GpEvent);
        if (Ret != 0)
        {
            ProcessEvent(Context, GameplayEventType.GameplayEventType_CloseGame);
            return -1;
        }

        GameplayStatusType GpNextStatusType = GetNextStatus(Context, GpCurStatus.GpType, GpEvent.Type);
        if (GpNextStatusType == GameplayStatusType.GameplayStatus_Invalid)
        {
            ProcessEvent(Context, GameplayEventType.GameplayEventType_CloseGame);
            return -1;
        }
        
        Context.CurStatusType = GpNextStatusType;
        GameplayAction GpNextAction = GetAction(Context, GpNextStatusType);
        return GpNextAction.OnEnter(Context);
    }
}
