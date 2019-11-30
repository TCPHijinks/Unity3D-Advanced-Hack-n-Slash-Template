using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAgility : Stat
{
    public override int GetModifiedLevel()
    {
        return creatureMod.StatAgilityMod;
    }
}
