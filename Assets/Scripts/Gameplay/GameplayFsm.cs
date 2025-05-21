using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayTransition
{
    private GameplayStatus gpStatus = GameplayStatus.GameplayStatus_Invalid;
    private GameplayEventType gpEventType;

    public GameplayStatus GpStatus { get => gpStatus; set => gpStatus = value; }
    public GameplayEventType GpEventType { get => gpEventType; set => gpEventType = value; }
}


public class GameplayFsm
{
    // 状态转换图
    private Dictionary<GameplayTransition, GameplayStatus> transitionMap;
    // action列表
    private Dictionary<GameplayStatus, GameplayAction> actionMap;

    public Dictionary<GameplayTransition, GameplayStatus> TransitionMap { get => transitionMap; set => transitionMap = value; }
    public Dictionary<GameplayStatus, GameplayAction> ActionMap { get => actionMap; set => actionMap = value; }

    // 初始化函数
    public void Init()
    {
        RegisterTransition(GameplayStatus.GameplayStatus_Invalid, GameplayEventType.GameplayEventType_StartGame, GameplayStatus.GameplayStatus_RoundStart);
    }

    private void RegisterTransition(GameplayStatus GpStatus, GameplayEventType GpEventType, GameplayStatus GpNewStatus)
    {
        GameplayTransition GpTransition = new GameplayTransition();
        GpTransition.GpStatus = GpStatus;
        GpTransition.GpEventType = GpEventType;
        TransitionMap.Add(GpTransition, GpNewStatus);
    }

}
