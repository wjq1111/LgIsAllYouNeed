using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEffect : BaseEffect
{
    public string Name = "default name";
    public int Attack = 0;
    public int Defense = 0;
    public int Hitpoint = 0;
    public int BaseAction = 0;
    public int BaseMovement = 0;

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
