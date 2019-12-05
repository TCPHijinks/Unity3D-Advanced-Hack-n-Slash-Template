using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEffect : Effect
{
    private float jmpAmount;
    public override void DoModify()
    {       
        creatureModifyable.JmpAmountEffectMod += jmpAmount;
    }

    public override void UndoModify()
    {
        creatureModifyable.JmpAmountEffectMod -= jmpAmount;
    }
}
