using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect
{
    private EffectType effectType = EffectType.EffectType_Invalid;
    public EffectType EffectType { get => effectType; set => effectType = value; }
}
