using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion 
{
    private int attack = 0;
    private int defense = 0;
    private int hitpoint = 0;

    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Hitpoint { get => hitpoint; set => hitpoint = value; }
}
