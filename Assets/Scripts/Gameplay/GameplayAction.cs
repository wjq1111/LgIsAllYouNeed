using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayAction
{

}

public class GameplayActionMgr : Singleton<GameplayActionMgr>
{
    private GameplayAction roundStartAction;
    private GameplayAction roundingAction;
    private GameplayAction roundEndAction;

    private Dictionary<GameplayStatus, GameplayAction> actionMap;

    public override void Init()
    {
        
    }
}