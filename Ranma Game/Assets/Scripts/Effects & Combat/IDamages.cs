using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamages 
{
    int WeaponTypeStandardBonusDamage { get; }
    int WeaponTypeHeavyBonusDamage { get; }
    int WeaponTypeChargedBonusDamage { get; }

    void DoDamage(CharModifyableProperties targetProperties, AttkType dmgType);
 
}


