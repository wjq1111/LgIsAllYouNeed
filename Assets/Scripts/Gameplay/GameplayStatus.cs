using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayStatus
{
    private long startTime = 0;
    private long timeoutTime = 0;
    private GameplayStatusType gpType = GameplayStatusType.GameplayStatus_Invalid;

    public long StartTime { get => startTime; set => startTime = value; }
    public long TimeoutTime { get => timeoutTime; set => timeoutTime = value; }
    public GameplayStatusType GpType { get => gpType; set => gpType = value; }
}
