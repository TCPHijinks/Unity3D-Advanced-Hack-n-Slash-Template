using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stops
/// </summary>
public class BlockMoveEffect : Effect
{
    private float curBaseMoveSpd = 0;
    private void Update()
    {        
      //  if (creatureModifyable.Creature.Blocking)
      //  {
     //       if (curBaseMoveSpd != creatureModifyable.Creature.BaseMoveStateMaxSpd)
     //       {              
     //           SetEffectOff();
     //           SetEffectOn();
    //        }
    //    }
    //    else if (!creatureModifyable.Creature.Blocking && TheEffectIsActive())
    //        SetEffectOff();
    }
    public override void DoModify()
    {        
        curBaseMoveSpd = creatureModifyable.Creature.BaseMoveStateMaxSpd;
        creatureModifyable.MaxSpdEffectMod -= curBaseMoveSpd / 2;
    }

    public override void UndoModify()
    {       
        creatureModifyable.MaxSpdEffectMod += curBaseMoveSpd / 2;
    }
}
