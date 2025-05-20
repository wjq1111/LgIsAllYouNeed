using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasBehavior : MonoBehaviour
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
        //var ButtonText = StartButton1.GetComponentsInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
