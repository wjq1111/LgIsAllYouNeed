using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 随从
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

    private bool oneRoundEffect = false;
    private AttrEffect attrEffect = new AttrEffect();

    public string Name { get => name; set => name = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Hitpoint
    {
        get => hitpoint;
        set
        { 
            hitpoint = value;
            // 只要有hp发生了变化，就去计算一次是不是可以游戏结束了
            GameObject BattleField = GameFramework.DfsObj(GameFramework.Instance.StartPrefab.transform, "BattleField").gameObject;
            BattleFieldBehaviour BattleFieldBehaviour = BattleField.GetComponent<BattleFieldBehaviour>();
            BattleFieldBehaviour.GameplayFsm.ProcessEvent(BattleFieldBehaviour.GameplayContext, GameplayEventType.GameplayEventType_CheckFinishGame);

            if (hitpoint <= 0)
            {
                GameFramework.Instance.BattleLog("dead effect");
            }
        }
    }
    public int RemainAction { get => remainAction; set => remainAction = value; }
    public int MaxAction { get => maxAction; set => maxAction = value; }
    public int RemainMovement { get => remainMovement; set => remainMovement = value; }
    public int MaxMovement { get => maxMovement; set => maxMovement = value; }
    public int PlayerId { get => playerId; set => playerId = value; }
    public bool OneRoundEffect { get => oneRoundEffect; set => oneRoundEffect = value; }
    public AttrEffect AttrEffect { get => attrEffect; set => attrEffect = value; }

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

    public void InitByMinionConfig(int minionId, int playerId)
    {
        MinionConfigItem MinionConfigItem = ConfigMgr.Instance.MinionConfig.MinionConfigMap[minionId];

        Attack = MinionConfigItem.Attack;
        Defense = MinionConfigItem.Defense;
        Hitpoint = MinionConfigItem.HitPoint;
        Name = MinionConfigItem.Name;
        RemainAction = MinionConfigItem.BaseAction;
        MaxAction = MinionConfigItem.BaseAction;
        RemainMovement = MinionConfigItem.BaseMovement;
        MaxMovement = MinionConfigItem.BaseMovement;
        PlayerId = playerId;
    }

    public void InitBySummonEffect(SummonEffect SummonEffect, int MyPlayerId)
    {
        Attack = SummonEffect.Attack;
        Defense = SummonEffect.Defense;
        Hitpoint = SummonEffect.Hitpoint;
        Name = SummonEffect.Name;
        RemainAction = SummonEffect.BaseAction;
        MaxAction = SummonEffect.BaseAction;
        RemainMovement = SummonEffect.BaseMovement;
        MaxMovement = SummonEffect.BaseMovement;
        PlayerId = MyPlayerId;
    }

    public void AddEffect()
    {
        if (AttrEffect.EffectAttrRoundType == EffectAttrRoundType.EffectAttrType_OneRound)
        {
            // 只加一回合，需要标记
            if (OneRoundEffect == false)
            {
                OneRoundEffect = true;
                RealAddEffect();
            }
        }
        else if (AttrEffect.EffectAttrRoundType == EffectAttrRoundType.EffectAttrType_EveryRound)
        {
            RealAddEffect();
        }
    }

    public void RealAddEffect()
    {
        if (AttrEffect.Attack != 0)
        {
            Attack += AttrEffect.Attack;
            GameFramework.Instance.BattleLog(Name + " attack add " + AttrEffect.Attack);
        }
        if (AttrEffect.Defense != 0)
        {
            Defense += AttrEffect.Defense;
            GameFramework.Instance.BattleLog(Name + " defense add " + AttrEffect.Defense);
        }
        if (AttrEffect.Hitpoint != 0)
        {
            Hitpoint += AttrEffect.Hitpoint;
            GameFramework.Instance.BattleLog(Name + " hitpoint add " + AttrEffect.Hitpoint);
        }
        if (AttrEffect.Action != 0)
        {
            MaxAction += AttrEffect.Action;
            GameFramework.Instance.BattleLog(Name + " max action add " + AttrEffect.Action);
        }
        if (AttrEffect.Movement != 0)
        {
            MaxMovement += AttrEffect.Movement;
            GameFramework.Instance.BattleLog(Name + " max movement add " + AttrEffect.Movement);
        }
    }
    public void RoundStartReset()
    {
        // 给Minion添加属性
        AddEffect();

        if (RemainAction != MaxAction || RemainMovement != MaxMovement)
        {
            // 回合开始时重置
            RemainAction = MaxAction;
            RemainMovement = MaxMovement;
            GameFramework.Instance.BattleLog(Name + " already reset!");
        }
    }
}
