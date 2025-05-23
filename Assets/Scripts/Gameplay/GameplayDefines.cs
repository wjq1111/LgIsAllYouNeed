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
    GameplayEventType_ClickCard,
    GameplayEventType_ClickTile,
    GameplayEventType_CloseGame,
    GameplayEventType_Timeout,
}

public enum GameplayEventResult
{
    GameplayEventResult_Invalid = 0,
    GameplayEventResult_NoNeedSwitch,
    GameplayEventResult_NeedSwitch,
}

public class GameplayEventClickCard
{
    public int PosX = 0;
    public int PosY = 0;
}

public class GameplayEventClickTile
{
    public int PosX = 0;
    public int PosY = 0;
}

public class GameplayEvent
{
    // 事件类型
    public GameplayEventType Type = GameplayEventType.GameplayEventType_Invalid;
    // 事件参数，后续按需添加
    public GameplayEventClickCard ClickCardEvent = new GameplayEventClickCard();
    public GameplayEventClickTile ClickTileEvent = new GameplayEventClickTile();

}