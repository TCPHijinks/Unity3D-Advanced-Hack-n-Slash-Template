using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : Creature
{
    private HumanoidAnim anim;              // Handles the animator.

    private Vector3 moveVel = Vector3.zero; // Direction to move.
    

    #region Movement.

    /// <summary>
    /// If do a Jump cooldown after just becoming grounded.
    /// </summary>  
    private bool DoJmpCd = false;

    /// <summary>
    /// Current acceleration/deceleration to 'dynamix max speed'.
    /// </summary>
    private float CurSpdToMax { get; set; }

    /// <summary>
    /// Amount to jump and following cooldown upon becoming grounded.
    /// </summary>
    [SerializeField] private float baseJmpAmount = 0.35f, jmpCd = 0.8f;
    /// <summary>
    /// Maximum jump height, calculated using 'base jump amount' and effect modifiers.
    /// </summary>
    private float JmpAmount
    {
        get { return baseJmpAmount + modifyableProperties.JmpAmountEffectMod; }
    }
    #endregion

    
    #region Combat

    /// <summary>
    /// If the humanoid is in a combat stance (i.e. weapon drawn).
    /// </summary>
    public bool InCombat { get; protected set; } = true;
    /// <summary>
    /// If the humanoid is currently blocking.
    /// </summary>
    public bool Blocking { get; protected set; } = false;
    /// <summary>
    /// Attacks can have combos in the animator. When Attack(), it increases to continue the combo.
    /// </summary>
    private int AttkComboStage { get; set; } = 0;
    #endregion
    

    /// <summary>
    /// Calculates and returns current horizontal humanoid velocity.
    /// </summary>
    private float HorizontalVel => .5f * Vector3.Magnitude(new Vector3(cControl.velocity.x, 0, cControl.velocity.z));

    /// <summary>
    /// Calculates modifier for terrain incline. ****** REDO ***********
    /// </summary>
    private float GndInclineSpdMod => (.33f * (gndCheck.GroundSlope * .4f)) * .15f;

    


    new void Awake()
    {        
        base.Awake();                  
        anim = GetComponent<HumanoidAnim>();
    }

       


    protected void Update()
    {
        SetMaxSpdTerrainMod();
        // Calculate current speed using velocity to reach max speed.
        SetCurSpdAccel(DynamicMaxSpd > 0);

        // Apply gravity to humanoid.
        GravityCalc(.032f);

        // Jump cooldown once after become grounded after being in air.
        StartCoroutine(JumpCooldown(jmpCd));
               
        
        // Apply in-air falling/jumping logic, then horizontal movement.
        JmpAndFallVelocity();
        ApplyMoveVelocity();

        // If grounded, reset vertical velocity after moving before next calc.
        if (gndCheck.Grounded) 
        {
            moveVel.x = 0;
            moveVel.z = 0;
        }
        else DoJmpCd = true; // Do a jump cooldown when next grounded.
       
        // Update animator's property values.
        AttackStageUpdate();
        anim.UpateHumanoidAnims(DoJmpCd, gndCheck.Grounded, Blocking, InCombat, _attkFin, CurSpdToMax, AttkComboStage);
    }




    /// <summary>
    /// Jump if grounded and not on cooldown.
    /// </summary>
    protected void Jump()
    {
        if (!DoJmpCd && gndCheck.Grounded) moveVel.y += JmpAmount;
    }




    bool _doingJmpCd = false; // Lock for cooldown so only run once when land.
    /// <summary>
    /// Set can't jump until cooldown time end.
    /// </summary>
    /// <param name="cooldown"></param>
    /// <returns>Player can jump again once complete.</returns>
    private IEnumerator JumpCooldown(float cooldown)
    {
        // Cooldown when just become grounded, and not already doing a cooldown.
        if (gndCheck.Grounded && DoJmpCd && !_doingJmpCd)
        {
            _doingJmpCd = true; // Is now on cooldown.

            yield return new WaitForSeconds(cooldown);
          
            DoJmpCd = false;    // Can jump now, don't do a cooldown until next landed.
            _doingJmpCd = false;// Not on cooldown anymore.
        }        
    }




    protected bool amLongJmping = false;               // If doing 'long jump' and Not a short one.
    [SerializeField] private float shortJmpFall = 10f; // Extra gravity while going up if doing 'short jump'.
    [SerializeField] private float normFallMod = 13f;  // Extra gravity when falling down.    
    private void JmpAndFallVelocity()
    {
        if (!gndCheck.Grounded)
        {
            // Stop accelerating up quicker if not long jumping.
            if (cControl.velocity.y > 0 && !amLongJmping)            
                moveVel.y -= shortJmpFall * Time.deltaTime;
            
            // Go higher if long jumping.
            else if (cControl.velocity.y > 0 && amLongJmping)
                moveVel.y += 1.5f * Time.deltaTime;

            // Fall faster if falling down.
            else if (cControl.velocity.y <= 0 && !gndCheck.Grounded)
                moveVel.y -= normFallMod * Time.deltaTime;
        }
    }




    [SerializeField] private float rotationSpdPenalty = .4f;
    /// <summary>
    /// Rotates humanoid to look at target position.
    /// </summary>
    /// <param name="newDir"></param>
    /// <param name="dampening"></param>
    /// <param name="rotSpeed"></param>
    protected void Rotate(Vector3 newDir, int dampening, int rotSpeed)
    {
        // Greater speed decreases turn speed.
        float spdRotPenalty = CurSpdToMax * rotationSpdPenalty; // Used on dampening & rotation speed.     

        Vector3 curMoveDir = Vector3.Lerp(transform.position, newDir, (dampening + spdRotPenalty));
        // Lerp from cur rot to new one.
        transform.rotation = (Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(new Vector3(curMoveDir.x, 0, curMoveDir.z)), (rotSpeed - spdRotPenalty) * Time.deltaTime));        
    }


    /// <summary>
    /// Sets 'terrain max speed modifier' penalty severity for 'Dynamic Max Speed'. 
    /// </summary>     
    public void SetMaxSpdTerrainMod() 
    {
        float newMax = -((int)gndCheck.GroundSlope * baseMoveStateMaxSpd / 100);
        if(Mathf.Abs(terrainMaxSpdMod - newMax) > .4f) terrainMaxSpdMod = newMax;
    }




    [SerializeField] private float curAccelToMax = 10;
    /// <summary>
    /// Returns current speed after applying either Acceleration or Deceleration.
    /// </summary>
    /// <param name="moving"></param>
    /// <param name="curSpeed"></param>    
    /// <returns></returns>
    private void SetCurSpdAccel(bool moving) 
    {        
        // If in air, limit speed.
        if (!gndCheck.Grounded)
        {
            CurSpdToMax = .3f + (Mathf.Clamp((HorizontalVel / 2) * .008f, 0, .015f));
            return;
        }         
             
        float acclBonus = 0;
        // If base max spd < dynamic MaxSpd, then going down slope so increase max spd.
        if ((DynamicMaxSpd - baseMoveStateMaxSpd) <= DynamicMaxSpd) // Base speed <= dynamic max speed (with modifiers)
             acclBonus += (HorizontalVel + GndInclineSpdMod) / 3; // Increase dynamic max speed (down slope).
        else acclBonus -= (HorizontalVel + GndInclineSpdMod) / 3; // Decrease it (up slope).
            
        // Accel if slower than max, decel if too fast.
        if (gndCheck.Grounded && moving && CurSpdToMax < DynamicMaxSpd)
            CurSpdToMax += (curAccelToMax + acclBonus) * Time.deltaTime;
        else if(CurSpdToMax > DynamicMaxSpd || !gndCheck.Grounded)
            CurSpdToMax -= (curAccelToMax + (acclBonus * 1.5f)) * Time.deltaTime;
        
        // Round down to max speed if close enough.
        if (Mathf.Abs(CurSpdToMax - DynamicMaxSpd) < .2 && CurSpdToMax > DynamicMaxSpd)
            CurSpdToMax = DynamicMaxSpd;
        else if (CurSpdToMax < 0)
            CurSpdToMax = 0;
    }
    



    /// <summary>
    /// Appply gravity to vertical move velocity.
    /// </summary>
    /// <param name="gravity"></param>
    private void GravityCalc(float gravity)
    {
        moveVel.y -= gravity;

        float terminalVel = 5 * -gravity;
        if (moveVel.y < terminalVel) // Enforce max fall velocity.
            moveVel.y = terminalVel;
        // Limit gravity when grounded.
        if (gndCheck.Grounded && moveVel.y < (terminalVel / 2)) 
            moveVel.y = terminalVel / 2;
    }

       


    /// <summary>
    /// Move humanoid character using move velocity.
    /// </summary>
    private void ApplyMoveVelocity()
    {
        // Apply horizontal movement.
        moveVel += (transform.forward * CurSpdToMax) * Time.deltaTime;

        // Move humanoid, applying all horizontal and vertical forces.        
        cControl.Move(moveVel);
    }




    private int _animAttkLayer;         // Animator layer thaht handles attacks.
    private float _followUpAttkTime = 0;// Time humanoid has to follow-up their attack to combo.
    private bool _attkFin = false, canCombo = false;
    [SerializeField] private float timeToFollowUpAttk = 4; 
    /// <summary>
    /// Increments attack combo state and resets timer to follower-up attack to prevent the combo (if any) breaking early.
    /// </summary>
    /// <param name="isCombo"></param>
    /// <param name="animAttkLayer"></param>
    protected void Attack(bool isCombo, int animAttkLayer)
    {        
        if (isCombo) canCombo = CurSpdToMax == 0;
       
        // Set anim layer that handles attk.
        _animAttkLayer = animAttkLayer;

        // If not just finished an attk combo, can attk.
        if (!_attkFin) 
        {
            // Set countdown window to allow follow-up attacks.
            _followUpAttkTime = timeToFollowUpAttk; 

            if (canCombo)        // If a combo, stack attack stages.
                AttkComboStage++;
            else AttkComboStage = 1;// Otherwise, default to first attack in combo.          
        }           
    }



    
    private bool _hasAttked = false;    
    /// <summary>
    /// Decrements follow-up attack countdown timer to end combo. 
    /// Reset's the attack state and updates whether the attack combo just finished for the animator.
    /// </summary>
    private void AttackStageUpdate()
    {
        // Time until attack cannot have a follow-up.
        _followUpAttkTime  -= 5 * Time.deltaTime;

        // Attk finished if attacked and ran out of time to follow-up or now idle.
        if (AttkComboStage > 0 && (_followUpAttkTime <= 0 || _hasAttked))
        {
            _followUpAttkTime = timeToFollowUpAttk;
            _attkFin = true;
            AttkComboStage = 0;
        }

        // If idle state and has just attacked.
        if (anim.LayerStateIdle(_animAttkLayer) && _attkFin)        
            _attkFin = false;            
        
        // If not a combo, reset combo stage once started first attk stage.
        if (!anim.LayerStateIdle(_animAttkLayer) && !canCombo)
            AttkComboStage = 0;

        // Has attked if now idle and finished an attack.
        _hasAttked = !anim.LayerStateIdle(_animAttkLayer) && _attkFin;
    }
}
