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
    private int remainMovement = 0;
    private int maxMovement = 0;

    private int playerId = 0;

    public string Name { get => name; set => name = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Hitpoint { get => hitpoint; set => hitpoint = value; }
    public int RemainAction { get => remainAction; set => remainAction = value; }
    public int MaxAction { get => maxAction; set => maxAction = value; }
    public int RemainMovement { get => remainMovement; set => remainMovement = value; }
    public int MaxMovement { get => maxMovement; set => maxMovement = value; }
    public int PlayerId { get => playerId; set => playerId = value; }

    public void Reset()
    {
        name = "default minion";
        attack = 0;
        defense = 0;
        hitpoint = 0;
        remainAction = 0;
        maxAction = 0;
        remainMovement = 0;
        maxMovement = 0;
        playerId = 0;
    }

    public void Copy(Minion NewMinion)
    {
        name = NewMinion.Name;
        attack = NewMinion.Attack;
        defense = NewMinion.Defense;
        hitpoint = NewMinion.Hitpoint;
        remainAction = NewMinion.RemainAction;
        maxAction = NewMinion.MaxAction;
        RemainMovement = NewMinion.RemainMovement;
        MaxMovement = NewMinion.MaxMovement;
        PlayerId = NewMinion.PlayerId;
    }
}
