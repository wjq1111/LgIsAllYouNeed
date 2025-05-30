using System;
using System.Collections;
using System.Collections.Generic;
using static System.DateTimeOffset;
using UnityEngine;
using UnityEditor.MPE;
using Unity.Mathematics;

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
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 10000;

        CardDeck.Instance.Init(Handbook.Instance.UnlockedCards, Context.GpFsm.CurPlayerIndex);
        return 0;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // ��ʱ
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
            GameFramework.Instance.BattleLog("Game Start!");

            GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
            BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
            BattleFieldBehaviour.GenerateBattleFieldTiles();
            BattleFieldBehaviour.InitPlayerObject();

            GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
            CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
            CardSessionBehaviour.GenerateCardSession();
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
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 2;
        if (Context.GpFsm.CurPlayerIndex > Context.Players.Count)
        {
            Debug.LogError("cur player index invalid " + Context.GpFsm.CurPlayerIndex);
            return -1;
        }

        // ϵͳ���ƣ�ֱ�Ӽӵ�player����
        GameObject CardSession = GameFramework.DfsObj(GameFramework.Instance.StartPrefab.transform, "CardSession").gameObject;
        CardSessionBehaviour CardSessionBehaviour = CardSession.GetComponent<CardSessionBehaviour>();

        int CurRoundDrawCardNum = GameFramework.Instance.CardNumPerRound;
        if (Context.Players[Context.GpFsm.CurPlayerIndex].CardList.Count + CurRoundDrawCardNum > CardSessionBehaviour.CardMaxCount)
        {
            CurRoundDrawCardNum = CardSessionBehaviour.CardMaxCount - Context.Players[Context.GpFsm.CurPlayerIndex].CardList.Count;
        }
        List<Card> CardList = CardDeck.Instance.DrawCards(CurRoundDrawCardNum);

        Context.Players[Context.GpFsm.CurPlayerIndex].AddCard(CardList);

        GameObject BattleField = GameFramework.Instance.GetBattleFieldObj();
        BattleFieldBehaviour BattleFieldBehaviour = BattleField.GetComponent<BattleFieldBehaviour>();
        // ����Minion�غ�reset����
        Dictionary<int, Dictionary<int, GameObject>> TileMap = GameFramework.Instance.GetBattleFieldObj().GetComponent<BattleFieldBehaviour>().TileMap;
        Dictionary<int, Dictionary<int, bool>> NeedRemoveMinion = new Dictionary<int, Dictionary<int, bool>>();
        foreach (var Dict in TileMap)
        {
            foreach (var Tile in Dict.Value)
            {
                Minion Minion = Tile.Value.GetComponent<BattleFieldTileBehaviour>().Minion;
                if (Minion != null && Minion.PlayerId == Context.GpFsm.CurPlayerIndex)
                {
                    Minion.RoundStartReset();
                    // ����ٻ����ǲ�������
                    if (Minion.Hitpoint <= 0)
                    {
                        if (NeedRemoveMinion.ContainsKey(Dict.Key) == false)
                        {
                            Dictionary<int, bool> keyValuePairs = new Dictionary<int, bool>();
                            keyValuePairs[Tile.Key] = true;
                            NeedRemoveMinion[Dict.Key] = keyValuePairs;
                        }
                        else
                        {
                            NeedRemoveMinion[Dict.Key].Add(Tile.Key, true);
                        }
                    }
                }
            }
        }
        // ����������ٻ���
        foreach (var PosX in NeedRemoveMinion.Keys)
        {
            foreach (var PosY in NeedRemoveMinion[PosX].Keys)
            {
                GameObject Tile = BattleFieldBehaviour.GetTile(PosX, PosY);
                GameFramework.Instance.BattleLog(Tile.GetComponent<BattleFieldTileBehaviour>().Minion.Name + " dead!");
                BattleFieldBehaviour.RemoveTileMinion(PosX, PosY);
            }
        }

        GameplayEvent GpEvent = new GameplayEvent();
        GpEvent.Type = GameplayEventType.GameplayEventType_DrawCard;
        Context.GpFsm.ProcessEvent(Context, GpEvent);

        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);
        if (GpEvent.Type == GameplayEventType.GameplayEventType_DrawCard)
        {
            int PlayerIndex = Context.GpFsm.CurPlayerIndex;
            if (PlayerIndex == (int)EPlayerIndex.RealPlayerIndex)
            {
                // ������Ⱦ��CardSession��
                Player CurPlayer = Context.Players[PlayerIndex];

                GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
                CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
                CardSessionBehaviour.ShowCards(CurPlayer.CardList);
            }
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }

        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // ��ʱ
            return -1;
        }
        // update
        return 0;
    }
}

public class RoundingAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        base.OnEnter(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        GpCurStatus.StartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        GpCurStatus.TimeoutTime = GpCurStatus.StartTime + 60;
        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);

        if (GpEvent.Type == GameplayEventType.GameplayEventType_EndRound)
        {
            GameFramework.Instance.BattleLog("cur player " + Context.GpFsm.CurPlayerIndex + " round end!");
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }

        if (GpEvent.Type == GameplayEventType.GameplayEventType_CheckFinishGame)
        {
            GameplayFinishReason Reason = GameFramework.Instance.GetBattleFieldObj().GetComponent<BattleFieldBehaviour>().NeedFinishGame();
            if (Reason != GameplayFinishReason.GameplayFinishReason_Invalid)
            {
                GameFramework.Instance.BattleLog("Game End!");
                return GameplayEventResult.GameplayEventResult_NeedSwitch;
            }
            return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
        }

        // ֻ����һغϵ������Ч��
        if (Context.GpFsm.CurPlayerIndex == (int)EPlayerIndex.RealPlayerIndex)
        {
            if (GpEvent.Type == GameplayEventType.GameplayEventType_ClickCard)
            {
                GameplayStatus GpCurStatus = GetCurStatus(Context);
                Player CurPlayer = Context.Players[Context.GpFsm.CurPlayerIndex];

                if (CurPlayer.CardList[GpEvent.ClickCardEvent.PosX].CardId != 0)
                {
                    CurPlayer.ChooseCardPosX = GpEvent.ClickCardEvent.PosX;
                    CurPlayer.ChooseCardPosY = GpEvent.ClickCardEvent.PosY;
                }
            }

            if (GpEvent.Type == GameplayEventType.GameplayEventType_ClickTile)
            {
                GameplayStatus GpCurStatus = GetCurStatus(Context);
                Player CurPlayer = Context.Players[Context.GpFsm.CurPlayerIndex];

                if (CurPlayer.ChooseCardPosX == -1 && CurPlayer.ChooseCardPosY == -1)
                {
                    // �����ѡ��Ŀ���λ���ǷǷ�ֵ��˵��û�е����ƣ�ֱ�ӵ���tile
                    // �ж�tile����û�����ӣ�����еĻ�������һ�ε�tile��ʱ���ƶ�����
                    if (CurPlayer.ChooseBattleFileTilePosX == -1 && CurPlayer.ChooseBattleFileTilePosY == -1)
                    {
                        CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                        CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                    }
                    else
                    {
                        GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                        BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                        // �����һ��ѡ��ĸ����ǿյģ�������ѡ��ĸ��ӣ������ͬ����Ϊû����
                        GameObject OldChooseTile = BattleFieldBehaviour.GetTile(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY);
                        Minion OldMinion = OldChooseTile.GetComponent<BattleFieldTileBehaviour>().Minion;
                        if (OldMinion == null)
                        {
                            // ��ѡ������ûѡ
                            CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                            CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                        }
                        else if (BattleFieldBehaviour.CanReach(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY))
                        {
                            // ��λ��֮��ľ���Ϸ�������λ��û�й���
                            Minion NewMinion = BattleFieldBehaviour.GetTile(GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY).GetComponent<BattleFieldTileBehaviour>().Minion;
                            if (NewMinion == null)
                            {
                                if (OldMinion.RemainMovement > 0)
                                {
                                    // ��ȥ��ԭλ�õ����ӣ��ټ�����λ�õ�����
                                    BattleFieldBehaviour.MoveTile(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY);
                                }
                                else
                                {
                                    // �ж������㣬��ʱ�����־������״̬Ӧ������ʾһ��
                                    Debug.Log(OldMinion.Name + " cannot move ");
                                }
                                CurPlayer.ResetChooseBattleFieldTile();
                            }
                            // ��λ���Ǽ����������ƶ�
                            else if (NewMinion.PlayerId == OldMinion.PlayerId)
                            {
                                Debug.Log(OldMinion.Name + " is blocked by " + NewMinion.Name);
                                CurPlayer.ResetChooseBattleFieldTile();
                            }
                            // ��λ���ǵз���ս��
                            else
                            {
                                if (OldMinion.RemainAction > 0)
                                {
                                    BattleFieldBehaviour.Combat(CurPlayer.ChooseBattleFileTilePosX, CurPlayer.ChooseBattleFileTilePosY, GpEvent.ClickTileEvent.PosX, GpEvent.ClickTileEvent.PosY);
                                    Debug.Log(OldMinion.Name + " fight with " + NewMinion.Name);
                                }
                                CurPlayer.ResetChooseBattleFieldTile();
                            }
                        }
                        else
                        {
                            // ����֮��ľ��벻�Ϸ����޷��ƶ�
                            CurPlayer.ChooseBattleFileTilePosX = GpEvent.ClickTileEvent.PosX;
                            CurPlayer.ChooseBattleFileTilePosY = GpEvent.ClickTileEvent.PosY;
                        }
                    }

                    return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
                }
                else
                {
                    // �����ѡ��Ŀ���λ�ò��ǷǷ�ֵ����Ϊ����Ѿ�ѡ�˿�������Ҫ�ŵ�ĳ��������
                    int TilePosX = GpEvent.ClickTileEvent.PosX;
                    int TilePosY = GpEvent.ClickTileEvent.PosY;

                    // �����ѡ���Ǹ��� ʩ�ӵ������ϵ�Ч��
                    GameObject BattleFieldObj = GameFramework.Instance.GetBattleFieldObj();
                    BattleFieldBehaviour BattleFieldBehaviour = BattleFieldObj.GetComponent<BattleFieldBehaviour>();
                    BattleFieldBehaviour.UseCard(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY, TilePosX, TilePosY);

                    // ɾ��������ƣ�����������Ⱦcard session
                    CurPlayer.DelCard(CurPlayer.ChooseCardPosX, CurPlayer.ChooseCardPosY);
                    GameObject CardSessionObj = GameFramework.Instance.GetCardSessionObj();
                    CardSessionBehaviour CardSessionBehaviour = CardSessionObj.GetComponent<CardSessionBehaviour>();
                    CardSessionBehaviour.ShowCards(CurPlayer.CardList);

                    CurPlayer.ResetChooseCard();
                }
            }
        }
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        base.OnUpdate(Context);
        GameplayStatus GpCurStatus = GetCurStatus(Context);
        if (GpCurStatus.TimeoutTime < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
        {
            // ��ʱ
            Context.GpFsm.ProcessEvent(Context, GameplayEventType.GameplayEventType_EndRound);
            return 0;
        }
        // update
        return 0;
    }
}

public class RoundEndAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        base.OnEnter(Context);
        Context.GpFsm.ProcessEvent(Context, GameplayEventType.GameplayEventType_NewRound);
        return 0;
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        base.OnEvent(Context, GpEvent);
        if (GpEvent.Type == GameplayEventType.GameplayEventType_NewRound)
        {
            return GameplayEventResult.GameplayEventResult_NeedSwitch;
        }
        return GameplayEventResult.GameplayEventResult_NoNeedSwitch;
    }

    public override int OnUpdate(GameplayContext Context)
    {
        return base.OnUpdate(Context);
    }

    public override int OnExit(GameplayContext Context, GameplayEvent GpEvent)
    {
        Context.GpFsm.CurPlayerIndex = (Context.GpFsm.CurPlayerIndex + 1) % Context.Players.Count;
        Debug.Log("cur player index -> " + Context.GpFsm.CurPlayerIndex);
        return 0;
    }
}

public class FinishGameAction : GameplayAction
{
    public override int OnEnter(GameplayContext Context)
    {
        return base.OnEnter(Context);
    }

    public override GameplayEventResult OnEvent(GameplayContext Context, GameplayEvent GpEvent)
    {
        return base.OnEvent(Context, GpEvent);
    }

    public override int OnUpdate(GameplayContext Context)
    {
        return base.OnUpdate(Context);
    }

    public override int OnExit(GameplayContext Context, GameplayEvent GpEvent)
    {
        return base.OnExit(Context, GpEvent);
    }
}



public class GameplayActionMgr : Singleton<GameplayActionMgr>
{
    private WaitStartAction waitStartAction = new WaitStartAction();
    private RoundStartAction roundStartAction = new RoundStartAction();
    private RoundingAction roundingAction = new RoundingAction();
    private RoundEndAction roundEndAction = new RoundEndAction();
    private FinishGameAction finishGameAction = new FinishGameAction();

    private Dictionary<GameplayStatusType, GameplayAction> actionMap = new Dictionary<GameplayStatusType, GameplayAction>();

    public new void Init()
    {
        actionMap.Add(GameplayStatusType.GameplayStatus_WaitStart, waitStartAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_RoundStart, roundStartAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_Rounding, roundingAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_RoundEnd, roundEndAction);
        actionMap.Add(GameplayStatusType.GameplayStatus_FinishGame, finishGameAction);
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