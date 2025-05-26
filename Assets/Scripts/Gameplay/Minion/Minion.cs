using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Ëæ´Ó
 */
public class Minion 
{
    private string name = "default minion";
    private int attack = 0;
    private int defense = 0;
    private int hitpoint = 0;

    private int remainAction = 0;
    private int maxAction = 0;

    public string Name { get => name; set => name = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Hitpoint { get => hitpoint; set => hitpoint = value; }
    public int RemainAction { get => remainAction; set => remainAction = value; }
    public int MaxAction { get => maxAction; set => maxAction = value; }

    public void Reset()
    {
        name = "default minion";
        attack = 0;
        defense = 0;
        hitpoint = 0;
        remainAction = 0;
        maxAction = 0;
    }

    public void Copy(Minion NewMinion)
    {
        name = NewMinion.Name;
        attack = NewMinion.Attack;
        defense = NewMinion.Defense;
        hitpoint = NewMinion.Hitpoint;
        remainAction = NewMinion.RemainAction;
        maxAction = NewMinion.MaxAction;
    }
}
