using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamages 
{
    int CrushingDamage { get; }
    int PiercingDamage { get; }
    int SlashingDamage { get; }

    void DoDamage(CreatureModifyableProperties targetProperties, AttkType dmgType, int amount);
 
}


