using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCharisma : Stat
{
    public override int GetModifiedLevel()
    {
        return creatureMod.StatCharismaMod;
    }
}
