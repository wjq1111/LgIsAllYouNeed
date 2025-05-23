using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSessionBehaviour : MonoSingleton<CardSessionBehaviour>
{
    private const int cardMaxCount = 5;
    private const float edgeLength = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCardSession();
    }

    private GameObject InstantiateCard(int PosX, int PosY)
    {
        float CanvasY = -593.0f;
        GameObject Prefab = Instantiate((GameObject)Resources.Load("Prefabs/Card"));
        Prefab.transform.SetParent(this.transform, false);
        float StartX = 200f;
        float StartY = 0f;
        float RealX = 1.732f * PosX * edgeLength + (PosY % 2) * 1.732f / 2 * edgeLength;
        float RealY = -470 - CanvasY + 1.5f * PosY * edgeLength;
        Prefab.transform.position = new Vector3(StartX + RealX, StartY + RealY, 0);
        TMP_Text PrefabText = Prefab.GetComponentInChildren<TMP_Text>();
        PrefabText.text = PosX + "-" + PosY;
        Prefab.name = "Card-" + PosX + "-" + PosY;

        CardBehaviour PrefabBehaviour = Prefab.GetComponent<CardBehaviour>();
        PrefabBehaviour.Name = Prefab.name;
        PrefabBehaviour.PosX = PosX;
        PrefabBehaviour.PosY = PosY;
        PrefabBehaviour.CoordX = StartX + RealX;
        PrefabBehaviour.CoordY = StartY + RealY + CanvasY;

        Button PrefabButton = Prefab.GetComponentInChildren<Button>();
        PrefabButton.onClick.AddListener(() => CardOnClick(PosX, PosY));


        return Prefab;
    }
    private void GenerateCardSession()
    {
        for (int i = 0; i < cardMaxCount; i++)
        {
            InstantiateCard(i, 0);
        }
    }


    public void ShowCards(List<Card> CardList)
    {
        // 先把手牌区所有card清空
        for (int i = 0; i < cardMaxCount; i++)
        {
            string Name = "Card-" + i + "-0";
            GameObject CardButton = GameFramework.DfsObj(GameFramework.Instance.StartPrefab.transform, Name).gameObject;
            TMP_Text CardButtonText = CardButton.GetComponentInChildren<TMP_Text>();
            CardButtonText.text = "Card-" + i + "-0";
        }
        for (int i = 0; i < CardList.Count; i++)
        {
            string Name = "Card-" + i + "-0";
            GameObject CardButton = GameFramework.DfsObj(GameFramework.Instance.StartPrefab.transform, Name).gameObject;
            TMP_Text CardButtonText = CardButton.GetComponentInChildren<TMP_Text>();

            Card Card = CardList[i];
            CardButtonText.text = Card.Name;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void CardOnClick(int PosX, int PosY)
    {
        GameObject BattleField = GameFramework.Instance.GetBattleFieldObj();
        BattleFieldBehaviour BattleFieldBehaviour = BattleField.GetComponent<BattleFieldBehaviour>();

        GameplayContext GpContext = BattleFieldBehaviour.GameplayContext;
        GameplayFsm GpFsm = BattleFieldBehaviour.GameplayFsm;

        Player CurPlayer = GpContext.Players[GpFsm.GetCurStatus(GpContext).CurPlayerIndex];
        if (PosX + 1 > CurPlayer.CardList.Count)
        {
            return;
        }


        GameplayEvent GpEvent = new GameplayEvent();
        GpEvent.Type = GameplayEventType.GameplayEventType_ClickCard;
        GpEvent.ClickCardEvent = new GameplayEventClickCard();
        GpEvent.ClickCardEvent.PosX = PosX;
        GpEvent.ClickCardEvent.PosY = PosY;
        GpFsm.ProcessEvent(GpContext, GpEvent);
    }
}
