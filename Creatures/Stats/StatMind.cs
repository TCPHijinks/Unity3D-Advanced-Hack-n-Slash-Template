using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMind : Stat
{
    public override int GetModifiedLevel()
    {
        return creatureMod.StatMindMod;
    }
}
