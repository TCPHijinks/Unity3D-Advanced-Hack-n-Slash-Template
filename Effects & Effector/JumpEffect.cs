using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEffect : Effect
{      
    public override void DoModify()
    {
        modAmount = 20;
        creatureModifyable.JmpAmountEffectMod += modAmount;
    }

    public override void UndoModify()
    {
        creatureModifyable.JmpAmountEffectMod -= modAmount;
    }
}
