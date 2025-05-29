using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEffect : BaseEffect
{
    private string name = "default name";
    private int attack = 0;
    private int defense = 0;
    private int hitpoint = 0;
    private int baseAction = 0;
    private int baseMovement = 0;
    public string Name { get => name; set => name = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Hitpoint { get => hitpoint; set => hitpoint = value; }
    public int BaseAction { get => baseAction; set => baseAction = value; }
    public int BaseMovement { get => baseMovement; set => baseMovement = value; }

    public void Init(MinionConfigItem MinionConfigItem)
    {
        Name = MinionConfigItem.Name;
        Attack = MinionConfigItem.Attack;
        Defense = MinionConfigItem.Defense;
        Hitpoint = MinionConfigItem.HitPoint;
        BaseAction = MinionConfigItem.BaseAction;
        BaseMovement = MinionConfigItem.BaseMovement;
    }
}
