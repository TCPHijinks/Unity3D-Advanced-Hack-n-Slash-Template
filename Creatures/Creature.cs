using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public CreatureModifyableProperties modifyableProperties;

    protected CharacterController cControl; // Movement handling.
    protected GroundCheck gndCheck;         // If grounded, incline checks, etc.    
    protected StatXPManager statXPAccess;
    protected void Awake()
    {
        modifyableProperties = GetComponent<CreatureModifyableProperties>();
        cControl = GetComponent<CharacterController>();
        gndCheck = GetComponent<GroundCheck>();
        statXPAccess = GetComponent<StatXPManager>();
    }


    #region Health, Magica, Stamina
    /// <summary>
    /// Base health points. Calculated using minimum HP and stat points.
    /// </summary>
    private int _baseMaxHp = 0;
    /// <summary>
    /// Maximum health. Calculated using base HP and 'effect modifiers' (e.g. armor).
    /// </summary>
   public int MaxHealth 
    { 
        get { return _baseMaxHp + modifyableProperties.MaxHpEffectMod; }
    }

    /// <summary>
    /// Base magica points. Calculated using minimum MP and stat points.
    /// </summary>
    private int _baseMaxMp = 0;
    /// <summary>
    /// Maximum magica. Calculated using base MP and 'effect modifiers' (e.g. magic staff).
    /// </summary>
    public int MaxMagica
    {
        get { return _baseMaxMp + modifyableProperties.MaxMpEffectMod; }
    }

    /// <summary>
    /// Base stamina points. Calculated using minimum SP and stat points.
    /// </summary>
    private int _baseMaxSp = 0;
    /// <summary>
    /// Maximum stamina. Calculated using base MP and 'effect modifiers' (e.g. shoes).
    /// </summary>
    public int MaxStamina
    {
        get { return _baseMaxSp + modifyableProperties.MaxSpEffectMod; }
    }
    #endregion


    #region Max Speed
    /// <summary>
    /// Used to calculate Dynamic Max Speed, by acting as the base movement speed set by a subclass creature (e.g. set it to 5 for running).
    /// </summary>
    /// 
    protected float baseMoveStateMaxSpd = 0;
    /// <summary>
    /// Used to calculate Dynamic Max Speed, by enabling the creature to faster/slower based on the terrain. 
    /// </summary>
    protected float terrainMaxSpdMod = 0;
    /// <summary>
    /// Returns base max speed in addition to environmental and effect modifiers whilst moving.
    /// </summary>
    protected float DynamicMaxSpd
    {        
        get 
        {
            if (baseMoveStateMaxSpd == 0) return 0; // No dynamic movement if not moving legs.
            return Mathf.Clamp((modifyableProperties.MaxSpdEffectMod + baseMoveStateMaxSpd) + terrainMaxSpdMod, 0, 99); 
        }      
    }
    #endregion


    #region Rotation
    /// <summary>
    /// Base speed of creature rotation before penalty, dampening and 'Rotation Speed Effects Modifier'.
    /// </summary>
    [SerializeField] private int baseRotSpd = 6;
        
    /// <summary>
    /// Active rotation speed of creature after 'Rotation Speed Effects Modifier' is added to the base speed.
    /// </summary>
    protected int RotationSpeed 
    {
        get
        {
            return baseRotSpd + modifyableProperties.RotSpdEffectsMod;
        }        
    }
    #endregion


   
}
