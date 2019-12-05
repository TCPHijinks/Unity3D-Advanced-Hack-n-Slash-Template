using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : Creature
{
    private HumanoidAnim anim;   
    
    #region Movement.
    /// <summary>
    /// Current acceleration/deceleration to 'dynamix max speed'.
    /// </summary>
    private float CurSpdToMax { get; set; }

    /// <summary>
    /// Amount to jump and following cooldown upon becoming grounded.
    /// </summary>
    [SerializeField] private float baseJmpAmount = 0.2f, jmpCd = 0.8f;
    /// <summary>
    /// Maximum jump height, calculated using 'base jump amount' and effect modifiers.
    /// </summary>
    private float JmpAmount
    {
        get { return baseJmpAmount + modifyableProperties.JmpAmountEffectMod; }
    }
    #endregion





    new void Awake()
    {        
        base.Awake();
        anim = GetComponent<HumanoidAnim>();
    }

    
       


    new void Update()
    {        
        base.Update();
        SetMaxSpdTerrainMod();
       
        SetCurSpdAccel(DynamicMaxSpd > 0); // Calc and apply cur accel.       
            
        JmpAndFallVelocity();

        MoveHumanoid();

        AttackStageUpdate();

        anim.UpateHumanoidAnims(canJmp, Grounded, Blocking, InCombat,  CurSpdToMax, AttkComboStage, BaseMoveStateMaxSpd == runMoveSpd, cmbtAnimDir.x, cmbtAnimDir.z);
    }
   




    /// <summary>
    /// Jump if grounded and not on cooldown.
    /// </summary>
    public void Jump()
    {      
        if (canJmp && Grounded)
        {            
            moveVel.y = JmpAmount;
            StartCoroutine(JumpCooldown(jmpCd));
        }
    }




    
    /// <summary>
    /// Set can't jump until cooldown time end.
    /// </summary>
    /// <param name="cooldown"></param>
    /// <returns>Player can jump again once complete.</returns>
    private IEnumerator JumpCooldown(float cooldown)
    {
        // Cooldown when just become grounded, and not already doing a cooldown.
        if (Grounded && canJmp)
        {
            canJmp = false;      
            yield return new WaitForSeconds(cooldown);
            canJmp = true;
        }        
    }
    private bool canJmp = true;




    private void JmpAndFallVelocity()
    {      
        if (!Grounded)
        {          
            // Stop accelerating up quicker if not long jumping.
            if (cControl.velocity.y > 0 && !amLongJmping)            
                moveVel.y -= shortJmpFall * Time.deltaTime;
            
            // Go higher if long jumping.
            else if (cControl.velocity.y > 0 && amLongJmping)
                moveVel.y += 1.5f * Time.deltaTime;

            // Fall faster if falling down.
            else if (cControl.velocity.y <= 0 && !Grounded)
                moveVel.y -= normFallMod * Time.deltaTime;
        }  
    }
    [HideInInspector] public bool amLongJmping = false;// If doing 'long jump' and Not a short one.
    [SerializeField] private float shortJmpFall = 10f; // Extra gravity while going up if doing 'short jump'.
    [SerializeField] private float normFallMod = 13f;  // Extra gravity when falling down.    





    public void SetMoveState(moveEnum moveState, Vector3 dirToMove)
    {
        cmbtAnimDir = dirToMove;
        if (dirToMove == Vector3.zero) cmbtMoveDir = Vector3.zero;
        else cmbtMoveDir = GetRelativeDir(dirToMove, transform);
        switch(moveState)
        {
            case moveEnum.Idle:
                BaseMoveStateMaxSpd = 0;
                break;
            case moveEnum.Walk:
                BaseMoveStateMaxSpd = defaultMoveSpd;
                break;
            case moveEnum.Jump:
                BaseMoveStateMaxSpd = JmpMoveSpd;
                break;
            default:
                BaseMoveStateMaxSpd = runMoveSpd;
                break;
        }
    }
    public enum moveEnum { Idle, Walk, Run, Jump }
    Vector3 cmbtMoveDir, cmbtAnimDir;





   

    public void RotateRelative(Vector3 lookDir, Transform relativeTo, int dampening)
    {       
        lookDir = GetRelativeDir(lookDir, relativeTo);
        ApplyRotation(lookDir, dampening);        
    }





    public void Rotate(Vector3 lookDir, int dampening)
    {        
        ApplyRotation(lookDir, dampening);
    }
    




    private void ApplyRotation(Vector3 lookDir, int dampening)
    {
        // Greater speed decreases turn speed.
        float spdRotPenalty = CurSpdToMax * rotationSpdPenalty; // Used on dampening & rotation speed.     

        Vector3 curMoveDir = Vector3.Lerp(transform.position, lookDir, (dampening + spdRotPenalty));
        // Lerp from cur rot to new one.
        transform.rotation = (Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(new Vector3(curMoveDir.x, 0, curMoveDir.z)), (RotationSpeed - spdRotPenalty) * Time.deltaTime));
    }
    [SerializeField] private float rotationSpdPenalty = .4f;





    /// <summary>
    /// Return movement direction using player input and camera rotation.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 GetRelativeDir(Vector3 direction, Transform relativeTo)
    {
        // Don't update if in air or not pressing key.
        if (direction == Vector3.zero) return transform.forward;

        // Movement dirs are relative to the player camera.
        Vector3 forward = relativeTo.forward.normalized;
        Vector3 right = relativeTo.right.normalized;

        // Return new movement direction relative to cam.
        return (forward * direction.z + right * direction.x);
    }





    /// <summary>
    /// Sets 'terrain max speed modifier' penalty severity for 'Dynamic Max Speed'. 
    /// </summary>     
    public void SetMaxSpdTerrainMod() 
    {
        float newMax = -((int)gndCheck.GroundSlope * BaseMoveStateMaxSpd / 100);
        if (Mathf.Abs(terrainMaxSpdMod - newMax) > .4f) terrainMaxSpdMod = newMax;
    }
    



    
    /// <summary>
    /// Returns current speed after applying either Acceleration or Deceleration.
    /// </summary>
    /// <param name="moving"></param> 
    /// <returns></returns>
    private void SetCurSpdAccel(bool moving) 
    {
        // Limit speed when blocking.
        if (Blocking && cmbtAnimDir.z < 0)
        {
            if (CurSpdToMax > DynamicMaxSpd * .75f)          
                CurSpdToMax = DynamicMaxSpd * .75f;            
        }
        else if (Blocking && cmbtAnimDir.x != 0)
        {
            if (CurSpdToMax > DynamicMaxSpd * .8f)
                CurSpdToMax = DynamicMaxSpd * .8f;
        }
            


        // Accel if cur spd slower than max, decel if too fast.
        if (Grounded && moving && CurSpdToMax < DynamicMaxSpd)
            CurSpdToMax += moveAccel * Time.deltaTime;
        else if(CurSpdToMax > DynamicMaxSpd)
            CurSpdToMax -= moveAccel * Time.deltaTime;
        
        // Round down to max speed if close enough.
        if (Mathf.Abs(CurSpdToMax - DynamicMaxSpd) < .2 && CurSpdToMax > DynamicMaxSpd)
            CurSpdToMax = DynamicMaxSpd;
        else if (CurSpdToMax < 0)
            CurSpdToMax = 0;
    }
    [SerializeField] private float moveAccel = 6f;





    /// <summary>
    /// Move humanoid character using move velocity.
    /// </summary>
    private void MoveHumanoid()
    {        
        // Apply horizontal movement.
        if(!Blocking || BaseMoveStateMaxSpd == runMoveSpd)
            moveVel += (transform.forward * CurSpdToMax) * Time.deltaTime;
        else
            moveVel += (cmbtMoveDir * CurSpdToMax) * Time.deltaTime;

        // Move humanoid, applying all horizontal and vertical forces.        
        cControl.Move(moveVel);

        // Reset horizontal force for next calc.      
        moveVel.x = 0;
        moveVel.z = 0;
    }




         
    /// <summary>
    /// Increments attack combo state and resets timer to follower-up attack to prevent the combo (if any) breaking early.
    /// </summary>
    /// <param name="isCombo"></param>
    public void Attack(bool isCombo)
    {
        if (!_canAttk) return;

        // No combo if moving.
        if (isCombo) isCombo = CurSpdToMax == 0;    

        // If not a combo, reset combo.
        if (!isCombo) AttkComboStage = 0;

        // Set countdown window to allow follow-up attacks.
        _followUpAttkTime = timeToFollowUpAttk; 
       
        // Move to next stage of combo in animator.
        AttkComboStage++;                      
    }
    private float _followUpAttkTime = 0;// Time humanoid has to follow-up their attack to combo.
    [SerializeField] private float timeToFollowUpAttk = 4;
   
    



    /// <summary>
    /// Decrements follow-up attack countdown timer to end combo. 
    /// </summary>
    private void AttackStageUpdate()
    {
        // Time until attack cannot have a follow-up.
        _followUpAttkTime  -= 5 * Time.deltaTime;

        // Reset combo if take too long to do follow-up attk.
        if (AttkComboStage > 0 && _followUpAttkTime <= 0)
        {
            _followUpAttkTime = timeToFollowUpAttk;       
            AttkComboStage = 0;
        }

        // Do CD to prevent anim canceling, If attk, then move.
        if(AttkComboStage > 0 && CurSpdToMax > 0)                  
            StartCoroutine(IdleToRunAttkCooldown(.4f));        
    }





    /// <summary>
    /// Can't attack again until cooldown finishes.
    /// </summary>
    /// <returns>Returns that can attk on cooldown finish.</returns>
    IEnumerator IdleToRunAttkCooldown(float length)
    {
        if (_canAttk)
        {       
            _canAttk = false;
            yield return new WaitForSeconds(length);
            _canAttk = true;          
        }        
    }
    bool _canAttk = true;
}
