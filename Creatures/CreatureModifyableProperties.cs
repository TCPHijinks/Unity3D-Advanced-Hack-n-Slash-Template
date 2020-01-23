using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modular exposure of modifyable variables specifically for IEffects usage.
/// </summary>
public class CreatureModifyableProperties : MonoBehaviour
{
    private CreatureAnimManager anim;
    public Creature Creature { get; private set; }
    public Health Health { get; private set; }
    private void Awake()
    {
        Creature = GetComponent<Creature>();
        Health = GetComponent<Health>();
        anim = GetComponent<CreatureAnimManager>();
    }

    public bool InStunState => anim.IsStunned;

    public void SetAnimStunned(Transform stunSrc, int StunStrength1to3)
    {
        anim.DoStun(stunSrc, transform, StunStrength1to3);
    }

    /// <summary>
    /// Maximum speed modifier for effects that affect how max a creature can move.
    /// </summary>
    [HideInInspector] public float MaxSpdEffectMod { get; set; } = 0;   // Modifiers use to increase/decrease max base speed.

    /// <summary>
    /// Modifier added to creature rotation speed before dampening and penalty. Used for Effects.
    /// </summary>
    [HideInInspector] public int RotSpdEffectsMod { get; set; }

    /// <summary>
    /// Modifier for maximum health points. Allows effects to increase/decrease a creature's max health.
    /// </summary>
    [HideInInspector] public int MaxHpEffectMod { get; set; } = 0;

    /// <summary>
    /// Modifier for maximum magica points. Allows effects to increase/decrease a creature's max magic.
    /// </summary>
    [HideInInspector] public int MaxMpEffectMod { get; set; } = 0;

    /// <summary>
    /// Modifier for maximum health points. Allows effects to increase/decrease a creature's max stamina.
    /// </summary>
    [HideInInspector] public int MaxSpEffectMod { get; set; } = 0;

    /// <summary>
    /// Modifier for increasing/decreasing the maximum jump height of a creature.
    /// </summary>
    [HideInInspector] public float JmpAmountEffectMod { get; set; } = 0;
   
    [HideInInspector] public float GravityEffectMod { get; set; } = 0;


    [HideInInspector] public int StatPhysicalMod { get; set; } = 0;
    [HideInInspector] public int StatCharismaMod { get; set; } = 0;
    [HideInInspector] public int StatAgilityMod { get; set; } = 0;
    [HideInInspector] public int StatMindMod { get; set; } = 0;
    
}
