public enum GameplayStatus
{
    GameplayStatus_Invalid = 0,
    GameplayStatus_RoundStart,
    GameplayStatus_Rounding,
    GameplayStatus_RoundEnd,
}

public enum GameplayEventType
{
    GameplayEventType_Invalid = 0,
    GameplayEventType_StartGame,
    GameplayEventType_DrawCard,
}

public class GameplayEvent
{
    // 事件类型
    private GameplayEventType type = GameplayEventType.GameplayEventType_Invalid;
    // 事件参数，后续按需添加
    private int param = 0;

    public GameplayEventType Type { get => type; set => type = value; }
    public int Param { get => param; set => param = value; }
}