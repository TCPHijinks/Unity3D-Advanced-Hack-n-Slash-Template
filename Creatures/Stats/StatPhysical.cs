using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPhysical : Stat
{
    public override int GetModifiedLevel()
    {
        return creatureMod.StatPhysicalMod;
    }
}
