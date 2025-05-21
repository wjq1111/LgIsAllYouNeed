using System.ComponentModel;

public enum GameplayStatusType
{
    GameplayStatus_Invalid = 0,
    GameplayStatus_WaitStart,
    GameplayStatus_RoundStart,
    GameplayStatus_Rounding,
    GameplayStatus_RoundEnd,
}

public enum GameplayEventType
{
    GameplayEventType_Invalid = 0,
    GameplayEventType_StartGame,
    GameplayEventType_DrawCard,
    GameplayEventType_CloseGame,
    GameplayEventType_Timeout,
}

public enum GameplayEventResult
{
    GameplayEventResult_Invalid = 0,
    GameplayEventResult_NoNeedSwitch,
    GameplayEventResult_NeedSwitch,
}

public class GameplayEvent
{
    // �¼�����
    private GameplayEventType type = GameplayEventType.GameplayEventType_Invalid;
    // �¼������������������
    private int param = 0;

    public GameplayEventType Type { get => type; set => type = value; }
    public int Param { get => param; set => param = value; }
}