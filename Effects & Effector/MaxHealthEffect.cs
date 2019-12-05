using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealthEffect : Effect
{
    private float modAmount;
    public override void DoModify()
    {
        creatureModifyable.MaxHpEffectMod += (int)modAmount;
    }

    public override void UndoModify()
    {
        creatureModifyable.MaxHpEffectMod -= (int)modAmount;
    }
}
