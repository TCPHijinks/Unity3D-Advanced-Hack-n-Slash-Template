using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    protected CreatureModifyableProperties modifyableProperties;
    protected CharacterController cControl; // Movement handling.
    protected GroundCheck gndCheck;         // If grounded, incline checks, etc.    
    protected StatXPManager statXPAccess;
    protected Vector3 moveVel = Vector3.zero; // Direction to move.





    protected void Awake()
    {
        modifyableProperties = GetComponent<CreatureModifyableProperties>();
        cControl = GetComponent<CharacterController>();
        gndCheck = GetComponent<GroundCheck>();
        statXPAccess = GetComponent<StatXPManager>();
    }





    protected void Update()
    {
        Grounded = gndCheck.Grounded;
        ApplyGravity();
    }
    public bool Grounded { private set; get; }




    /// <summary>
    /// Dynamic jump speed gives speed bonus for creature velocity.
    /// </summary>
    protected float JmpMoveSpd 
    { 
        get 
        {
            float HorizontalVel = Vector3.Magnitude(new Vector3(cControl.velocity.x, 0, cControl.velocity.z)) * 12;
            return (DynamicMaxSpd * 4 + HorizontalVel) * 8f;
        } 
    }
    [SerializeField] protected float defaultMoveSpd = 2.5f, runMoveSpd = 4f;





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
    public float BaseMoveStateMaxSpd { get; protected set; } = 0;
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
            float maxSpdBaseNMod = modifyableProperties.MaxSpdEffectMod + BaseMoveStateMaxSpd;

            if (BaseMoveStateMaxSpd == 0) return 0; // No dynamic movement if not moving legs.
            if (Blocking && BaseMoveStateMaxSpd != runMoveSpd) return Mathf.Clamp(maxSpdBaseNMod * .75f + terrainMaxSpdMod / 2, 0, 99);
            return Mathf.Clamp(maxSpdBaseNMod + terrainMaxSpdMod, 0, 99); 
        }      
    }
    #endregion





    #region Rotation
    [SerializeField] private int baseRotSpd = 6;
    /// <summary>
    /// Base speed of creature rotation before penalty, dampening and 'Rotation Speed Effects Modifier'.
    /// </summary>
    public int BaseRotSpd { get { return baseRotSpd; } private set { baseRotSpd = value; } }
        
    /// <summary>
    /// Active rotation speed of creature after 'Rotation Speed Effects Modifier' is added to the base speed.
    /// </summary>
    public int RotationSpeed 
    {
        get
        {            
            return Mathf.Clamp(BaseRotSpd + modifyableProperties.RotSpdEffectsMod, 0, 99);
        }        
    }
    #endregion





    #region Combat
    /// <summary>
    /// If the humanoid is in a combat stance (i.e. weapon drawn).
    /// </summary>
    public bool InCombat { get; set; } = false;
    /// <summary>
    /// If the humanoid is currently blocking.
    /// </summary>
    public bool Blocking { get; set; } = false;
    /// <summary>
    /// Attacks can have combos in the animator. When Attack(), it increases to continue the combo.
    /// </summary>
    protected int AttkComboStage { get; set; } = 0;
    #endregion





    #region Gravity
    public void ApplyGravity() 
    {
        moveVel.y -= Gravity;

        float terminalVel = 3 * -Gravity;
        if (moveVel.y < terminalVel) // Enforce max fall velocity.
            moveVel.y = terminalVel;
        // Limit gravity when grounded.
        if (Grounded && moveVel.y < terminalVel)
            moveVel.y = terminalVel;        
    }

    public float Gravity 
    { 
        get { return _baseGravity + modifyableProperties.GravityEffectMod; } 
    }
    private readonly float _baseGravity = 0.032f;
    #endregion
}
