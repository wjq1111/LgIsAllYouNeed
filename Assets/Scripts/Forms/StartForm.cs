using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class StartForm : MonoSingleton<StartForm>
{
    public Button StartButton1;
    public Button EndButton1;

    public GameObject BattleField;
    
    // Start is called before the first frame update
    void Start()
    {
        //TMP_Text StartText = (TMP_Text)StartButton1.gameObject.GetComponent("Text");
        //Debug.LogFormat("{0}", StartText);
        //StartText.text = "LG Start Game";
        StartButton1.GetComponentInChildren<TMP_Text>().text = "Here LG comes";
        EndButton1.GetComponentInChildren<TMP_Text>().text = "There LG goes";

        StartButton1.gameObject.SetActive(true);
        EndButton1.gameObject.SetActive(true);
        //var ButtonText = StartButton1.GetComponentsInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnChoiceSelected(int ChoiceIndex)
    {
        BattleFieldBehaviour BattleFieldBehaviour = (BattleFieldBehaviour)BattleField.GetComponent<BattleFieldBehaviour>();
        BattleFieldBehaviour.GameplayFsm.ProcessEvent(BattleFieldBehaviour.GameplayContext, GameplayEventType.GameplayEventType_StartGame);

        if (ChoiceIndex == 0)
        {
            Debug.LogFormat("LG Comes!!!");
        }
        else if (ChoiceIndex == 1)
        {
            Debug.LogFormat("LG Goes!!!");
        }
        else
        {
            Debug.LogFormat("Unknown Selection!!!!!!!!!!!!!!");
        }

        StartButton1.gameObject.SetActive(false);
        EndButton1.gameObject.SetActive(false);
    }
}
