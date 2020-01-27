using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamages 
{
    int WeaponTypeCrushBonusDamage { get; }
    int WeaponTypePierceBonusDamage { get; }
    int WeaponTypeSlashBonusDamage { get; }

    void DoDamage(CharModifyableProperties targetProperties, AttkType dmgType);
 
}


