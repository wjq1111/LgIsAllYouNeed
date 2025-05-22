using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        foreach (Card card in CardList)
        {
            Debug.Log("show card " + card.Gid);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
