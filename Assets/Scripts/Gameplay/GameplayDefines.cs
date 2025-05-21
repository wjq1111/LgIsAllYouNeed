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
    // �¼�����
    private GameplayEventType type = GameplayEventType.GameplayEventType_Invalid;
    // �¼������������������
    private int param = 0;

    public GameplayEventType Type { get => type; set => type = value; }
    public int Param { get => param; set => param = value; }
}