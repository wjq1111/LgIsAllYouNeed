using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class StartForm : MonoSingleton<StartForm>
{
    public Button StartButton1;
    public Button EndButton1;
    
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
        if (ChoiceIndex == 0) Debug.LogFormat("LG Comes!!!");
        else if (ChoiceIndex == 1) Debug.LogFormat("LG Goes!!!");
        else Debug.LogFormat("Unknown Selection!!!!!!!!!!!!!!");

        StartButton1.gameObject.SetActive(false);
        EndButton1.gameObject.SetActive(false);
    }
}
