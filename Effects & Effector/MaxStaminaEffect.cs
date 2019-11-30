using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxStaminaEffect : Effect
{    
    public override void DoModify()
    {
        creatureModifyable.MaxSpEffectMod += (int)modAmount;
    }

    public override void UndoModify()
    {
        creatureModifyable.MaxSpEffectMod -= (int)modAmount;
    }
}
