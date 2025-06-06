using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    private new string name = new string("default name");
    private int posX = 0;
    private int posY = 0;

    private float coordX = 0;
    private float coordY = 0;

    public string Name { get => name; set => name = value; }
    public int PosX { get => posX; set => posX = value; }
    public int PosY { get => posY; set => posY = value; }
    public float CoordX { get => coordX; set => coordX = value; }
    public float CoordY { get => coordY; set => coordY = value; }

    public string Log()
    {
        return "card: " + Name + " pos x: " + PosX + " pos y: " + PosY + " coord x: " + coordX + " coord y: " + coordY;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
