using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modular exposure of modifiable variables specifically for IEffects usage.
/// </summary>
public class CharModifyableProperties : MonoBehaviour
{
    public Character character;
    public CharacterAnimManager AnimManager;
    public Health Health { get; private set; }
    private void Awake()
    {
        AnimManager = GetComponentInChildren<CharacterAnimManager>();
        character = GetComponent<Character>();
        Health = GetComponent<Health>();    
    }

    

   

    /// <summary>
    /// Maximum speed modifier for effects that affect how max a creature can move.
    /// </summary>
    [HideInInspector] public float MaxSpeedMod { get; set; } = 0;   // Modifiers use to increase/decrease max base speed.
       
    /// <summary>
    /// Modifier for maximum health points. Allows effects to increase/decrease a creature's max health.
    /// </summary>
    [HideInInspector] public int MaxHealthMod { get; set; } = 0;
       

    /// <summary>
    /// Modifier for maximum health points. Allows effects to increase/decrease a creature's max stamina.
    /// </summary>
    [HideInInspector] public int MaxStaminaMod { get; set; } = 0;

    /// <summary>
    /// Modifier for increasing/decreasing the maximum jump height of a creature.
    /// </summary>
    [HideInInspector] public float JmpAmountEffectMod { get; set; } = 0;
   
   


    [HideInInspector] public int StatPhysicalMod { get; set; } = 0;
    [HideInInspector] public int StatCharismaMod { get; set; } = 0;
    [HideInInspector] public int StatAgilityMod { get; set; } = 0;
    [HideInInspector] public int StatMindMod { get; set; } = 0;
    
}
