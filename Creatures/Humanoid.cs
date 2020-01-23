using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : Creature
{
    #region Movement.
    /// <summary>
    /// Current acceleration/deceleration to 'dynamix max speed'.
    /// </summary>
    public float CurSpeed { get; private set; }

    /// <summary>
    /// Amount to jump and following cooldown upon becoming grounded.
    /// </summary>
    [SerializeField] private float baseJumpAmount = 0.2f, jumpCooldown = 0.8f;
    /// <summary>
    /// Maximum jump height, calculated using 'base jump amount' and effect modifiers.
    /// </summary>
    private float JmpAmount
    {
        get { return baseJumpAmount + modifyableProperties.JmpAmountEffectMod; }
    }
    #endregion





    new void Awake()
    {        
        base.Awake();        
    }

   
    public bool IsAttacking { get; private set; }

    void Update()
    {       
        SetMaxSpdTerrainMod();
       
        SetCurSpdAccel(); // Calc and apply cur accel.       
                  
        MoveHumanoid();
    }

   

    public void PreventClipping(string otherMaskLayer, int numChecks)
    {
        RaycastHit hit;
        LayerMask otherLayer = LayerMask.NameToLayer(otherMaskLayer);

        //Bottom of controller. Slightly above ground so it doesn't bump into slanted platforms. (Adjust to your needs)
        Vector3 p1 = transform.position + Vector3.up * 0.25f;
        //Top of controller
        Vector3 p2 = p1 + Vector3.up * cControl.height;

        //Check around the character in a 360, 10 times (increase if more accuracy is needed)
        for (int i = 0; i < numChecks; i += 36)
        {
            //Check if anything with the platform layer touches this object
            if (Physics.CapsuleCast(p1, p2, 0, new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)), out hit, distance, 1 << otherLayer))
            {
                //If the object is touched by a platform, move the object away from it
                cControl.Move(hit.normal * (distance - hit.distance));
            }
        }

        //[Optional] Check the players feet and push them up if something clips through their feet.
        //(Useful for vertical moving platforms)
        if (Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out hit, 1, 1 << otherLayer))
        {
            cControl.Move(Vector3.up * (1 - hit.distance));
        }
    }

    //Distance is slightly larger than the
    float distance => cControl.radius + 0.2f;
 
        //First add a Layer name to all platforms (I used MovingPlatform)
        //Now this script won't run on regular objects, only platforms.
     // => gameObject.tag == "Enemy" ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");


    public void SetMoveState(moveEnum moveState)
    {
        switch(moveState)
        {
            case moveEnum.Idle:
                BaseMoveStateMaxSpd = 0;
                break;
            case moveEnum.Walk:
                BaseMoveStateMaxSpd = WalkMoveSpd;
                break;
            case moveEnum.Jump:
                BaseMoveStateMaxSpd = JmpMoveSpd;
                break;
            case moveEnum.Sneak:
                BaseMoveStateMaxSpd = SneakMoveSpd;
                break;
            default:
                BaseMoveStateMaxSpd = RunMoveSpd;
                break;
        }
    }
    public enum moveEnum { Idle, Walk, Run, Sneak, Jump }






   

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
        float spdRotPenalty = CurSpeed * rotationSpdPenalty; // Used on dampening & rotation speed.     

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
    /// Calcs & sets current speed. Applying either accel or decel if needed.
    /// </summary>        
    private void SetCurSpdAccel() 
    {
        // Calc effective max speed & don't calc accel if already at it.        
        if (CurSpeed == DynamicMaxSpd) return;
        

        // Accel if cur spd slower than max, decel if too fast.
        if (Grounded  && CurSpeed <= DynamicMaxSpd && BaseMoveStateMaxSpd != (int)moveEnum.Idle)
            CurSpeed += moveAccel * Time.deltaTime;
        else if (CurSpeed > DynamicMaxSpd || BaseMoveStateMaxSpd == (int)moveEnum.Idle)
            CurSpeed -= (moveAccel * 1.25f) * Time.deltaTime;


        // Round to max speed if close enough.
        if (Mathf.Abs(CurSpeed - DynamicMaxSpd) < .3)
            CurSpeed = DynamicMaxSpd;         
    }
    [SerializeField] private float moveAccel = 10f;





    /// <summary>
    /// Move humanoid character using move velocity.
    /// </summary>
    private void MoveHumanoid()
    {        
        ////////////// Apply horizontal movement.
      ///////////////  if((!InCombat) || BaseMoveStateMaxSpd == runMoveSpd)
            //moveVel += (transform.forward * CurSpdToMax) * Time.deltaTime;
       ///////////// else
       /////////////     moveVel += (cmbtMoveDir * CurSpdToMax) * Time.deltaTime;

        // Move humanoid, applying all horizontal and vertical forces.        
        cControl.Move(moveVel);

        // Reset horizontal force for next calc.      
        moveVel.x = 0;
        moveVel.z = 0;
    }
}
