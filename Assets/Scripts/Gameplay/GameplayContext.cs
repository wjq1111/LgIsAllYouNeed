using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContext
{
    private GameplayStatus waitStartStatus = new GameplayStatus();
    private GameplayStatus roundStartStatus = new GameplayStatus();
    private GameplayStatus roundingStatus = new GameplayStatus();
    private GameplayStatus roundEndStatus = new GameplayStatus();

    private Dictionary<GameplayStatusType, GameplayStatus> gpStatusMap = new Dictionary<GameplayStatusType, GameplayStatus>();

    private GameplayStatusType curStatusType = GameplayStatusType.GameplayStatus_Invalid;

    public GameplayStatusType CurStatusType { get => curStatusType; set => curStatusType = value; }

    public void Init()
    {
        waitStartStatus.GpType = GameplayStatusType.GameplayStatus_WaitStart;
        roundStartStatus.GpType = GameplayStatusType.GameplayStatus_RoundStart;
        roundingStatus.GpType = GameplayStatusType.GameplayStatus_Rounding;
        roundEndStatus.GpType = GameplayStatusType.GameplayStatus_RoundEnd;

        gpStatusMap.Add(waitStartStatus.GpType, waitStartStatus);
        gpStatusMap.Add(roundStartStatus.GpType, roundStartStatus);
        gpStatusMap.Add(roundingStatus.GpType, roundingStatus);
        gpStatusMap.Add(roundEndStatus.GpType, roundEndStatus);
    }

    public GameplayStatus GetStatus(GameplayStatusType GpStatus)
    {
        if (gpStatusMap.ContainsKey(GpStatus) == false)
        {
            Debug.LogError("status map not contains type:" + GpStatus);
            return null;
        }
        return gpStatusMap[GpStatus];
    }
}
