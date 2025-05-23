using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Card
{
    //private int cid = 0;
    private int gid = 0;
    private string name = "default card";
    private List<BaseEffect> cardEffect = new List<BaseEffect>();

    //public int Cid { get => cid; set => cid = value; }
    public int Gid { get => gid; set => gid = value; }
    public string Name { get => name; set => name = value; }

    public void Init(string Name, List<BaseEffect> cardEffect)
    {
        //this.cid = cid;
        this.Name = Name;
        this.cardEffect = cardEffect;
    }
}
