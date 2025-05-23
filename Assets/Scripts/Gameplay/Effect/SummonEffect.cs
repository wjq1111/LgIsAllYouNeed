using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEffect : BaseEffect
{
    private int attack = 0;
    private int defense = 0;
    private int hitpoint = 0;

    // TODO: minion effect list

    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Hitpoint { get => hitpoint; set => hitpoint = value; }

    //public SummonEffect(int Minionid) 
    //{ 
    //    this.minionid = Minionid;
    //}

    public void Init(int attack, int defense, int hitpoint)
    {
        this.attack = attack;
        this.defense = defense;
        this.hitpoint = hitpoint;
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
