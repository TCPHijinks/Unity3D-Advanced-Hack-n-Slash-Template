using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotateEffect : Effect
{
    private int rotSpd = 0;
    private void Update()
    {
        if (creatureModifyable.Creature.Blocking)        
            SetEffectOn();                    
        else if (!creatureModifyable.Creature.Blocking)
            SetEffectOff();
    }
    public override void DoModify()
    {
        rotSpd = creatureModifyable.Creature.RotationSpeed * 2;
        creatureModifyable.RotSpdEffectsMod -= rotSpd;
        Debug.Log(creatureModifyable.RotSpdEffectsMod);
    }

    public override void UndoModify()
    {

        creatureModifyable.RotSpdEffectsMod += rotSpd;
        Debug.Log("UNDO:"+creatureModifyable.RotSpdEffectsMod);
    }
}
