using System.ComponentModel;

public enum GameplayStatusType
{
    GameplayStatus_Invalid = 0,
    GameplayStatus_WaitStart,
    GameplayStatus_RoundStart,
    GameplayStatus_Rounding,
    GameplayStatus_RoundEnd,
    GameplayStatus_FinishGame,
}

public enum GameplayEventType
{
    GameplayEventType_Invalid = 0,
    GameplayEventType_StartGame,
    GameplayEventType_DrawCard,
    GameplayEventType_ClickCard,
    GameplayEventType_ClickTile,
    GameplayEventType_EndRound,
    GameplayEventType_NewRound,
    GameplayEventType_CloseGame,
    GameplayEventType_CheckFinishGame,
    GameplayEventType_Timeout,
}

public enum GameplayEventResult
{
    GameplayEventResult_Invalid = 0,
    GameplayEventResult_NoNeedSwitch,
    GameplayEventResult_NeedSwitch,
}

public enum GameplayFinishReason
{
    GameplayFinishReason_Invalid = 0,
    GameplayFinishReason_RealPlayerWin,
    GameplayFinishReason_EnemyPlayerWin,
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
    // �¼�����
    public GameplayEventType Type = GameplayEventType.GameplayEventType_Invalid;
    // �¼������������������
    public GameplayEventClickCard ClickCardEvent = new GameplayEventClickCard();
    public GameplayEventClickTile ClickTileEvent = new GameplayEventClickTile();

}

public enum EPlayerIndex
{
    RealPlayerIndex = 0,
    EnemyPlayerIndex = 1,
}

public enum EffectType
{
    EffectType_Invalid = 0,
    // �ٻ�
    EffectType_Summon = 1,
    // �޸��������
    EffectType_Attr = 2,
}

public enum EffectAttrRoundType
{
    EffectAttrType_Invalid = 0,
    EffectAttrType_OneRound = 1,
    EffectAttrType_EveryRound = 2,
}