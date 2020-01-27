using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : Effect
{
    protected override void DoModify()
    {
        creatureModifyable.Health.TakeDamage((int)amount);
        Destroy(this);
    }

    protected override void UndoModify()
    {
        Debug.LogError("Error. Nothing to undo.");
    }
}
