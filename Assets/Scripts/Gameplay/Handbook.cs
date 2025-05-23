using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Handbook : Singleton<Handbook>
{
    private Dictionary<int, Card> cards = new Dictionary<int, Card>();

    public new void Init()
    {
        Card dummy = new Card();
        List<BaseEffect> effects = new List<BaseEffect>();

        SummonEffect summonEffect = new SummonEffect();
        summonEffect.Init(1, 1, 1);
        effects.Add(summonEffect);

        dummy.Init("Gaowenchang", effects);
        cards.Add(1, dummy);
    }
}
