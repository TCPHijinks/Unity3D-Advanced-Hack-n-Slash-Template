using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stat : MonoBehaviour
{
    protected CreatureModifyableProperties creatureMod; 
    private Dictionary<int, int> LevAndXpThreshold = new Dictionary<int, int>();
    private int curExperience = 0;

    protected int CurLevel
    {
        get
        {
            return GetBaseLevel() + GetModifiedLevel();
        }
    }
       

    void Awake()
    {       
        int levThreshold = 600;

        // Setup level thresholds 0,600,1200...307200.
        for (int i = 0; i < 10; i++)
        {
            if(i == 0) LevAndXpThreshold.Add(i, 0);
            else
            {
                levThreshold *= 2;
                LevAndXpThreshold.Add(i, levThreshold);
            }
        }
    }

    private int GetBaseLevel()
    {
        // Try all 10 level thresholds.
        for(int i = 0; i < 10; i++)
        {
            // Return the level if xp cur xp not exceed threshold.
            if (curExperience <= LevAndXpThreshold[i])
                return i;
        }
        return 10; // Enforce not exceed max level.
    }


    public abstract int GetModifiedLevel();
    

    public void GainExperience(int XP)
    {
        if (curExperience >= LevAndXpThreshold[9]) return;

        XP = Mathf.Clamp(XP, 0, LevAndXpThreshold[9] - curExperience);
        curExperience += XP;
    }

    public void LoseExperience(int XP)
    {
        XP = Mathf.Clamp(XP, 0, curExperience);
        curExperience -= XP;
    }
}
