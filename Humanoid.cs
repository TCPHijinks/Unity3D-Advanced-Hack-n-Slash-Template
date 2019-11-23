using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : MonoBehaviour
{
    protected GroundCheck gndCheck; // If grounded, incline checks, etc.
    protected CharacterController controller; // Movement handling.
    private HumanoidMoveStd stdMovement;      // Handles generall non-combat movement.

    public float Gravity = .032f;// { get; set; } = .032f;
 
    [HideInInspector] public bool StartedJmpCd { get; private set; } = false; // Only allow one active jump cooldown.
    [HideInInspector] public bool CanJump { get; private set; } = true; // Jump cooldown lock.
    [HideInInspector] public float CurSpd { get; private set; }
    [HideInInspector] public float MaxSpd { get; private set; }

    public float jogSpd = 1.45f, runSpd = 2f;
    public float jumpAmount = 0.35f, jumpCooldown = 0.8f;
    public float accelMod = 10; // Used to calculate cur speed.


    private float _baseMaxSpd;

    void Awake()
    {
        gndCheck = GetComponent<GroundCheck>(); 
        controller = GetComponent<CharacterController>();
        stdMovement = GetComponent<HumanoidMoveStd>();
    }


    // Update is called once per frame
    protected void Update()
    {      
        CurSpd = GetCurSpdAccel(MaxSpd > 0, CurSpd);
        GravityCalc(Gravity);

        if (gndCheck.Grounded && !CanJump) // Start jump cooldown once grounded.
            StartCoroutine(JumpCooldown(jumpCooldown));
        
        moveVel += (transform.forward * CurSpd) * Time.deltaTime;

        ApplyMoveVelocity();

        if (gndCheck.Grounded) // If grounded, limit move speed.
        {
            moveVel.x = 0;
            moveVel.z = 0;
        }    
        else CanJump = false; // Can't jump if not grounded. 
    }

    [SerializeField] private float rotSpdPenalty = .4f;
    /// <summary>
    /// Rotates humanoid to look at target position.
    /// </summary>
    /// <param name="newDir"></param>
    /// <param name="dampening"></param>
    /// <param name="rotSpeed"></param>
    protected void Rotate(Vector3 newDir, int dampening, int rotSpeed)
    {
        // Greater speed decreases turn speed.
        float spdRotPenalty = CurSpd * rotSpdPenalty; // Used on dampening & rotation speed.     
        
        Vector3 curMoveDir = Vector3.Lerp(transform.position, newDir, (dampening + spdRotPenalty));
        // Lerp from cur rot to new one.
        transform.rotation = (Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(new Vector3(curMoveDir.x, 0, curMoveDir.z)), (rotSpeed - spdRotPenalty) * Time.deltaTime));
    }


    private float CurVel => .5f * Vector3.Magnitude
        (new Vector3(controller.velocity.x, 0, controller.velocity.z));     // Horizontal velocity.
    private float GndSpdMod => (.33f * (gndCheck.GroundSlope * .4f)) * .15f; // Terrain incline/decline speed modifier.
    private Vector3 moveVel = Vector3.zero;                                 // Direction to move.

    private float _minMvSpd = .2f;
    private float _prevMaxSpd = 0;
    /// <summary>
    /// Sets new dynamic max speed to allow movement.
    /// </summary>
    /// <param name="mxSpeed"></param>
    protected void Move(float mxSpeed)
    {
        // Update max speed for standard movement, and save original base max move speed for acceleration.
        MaxSpd = stdMovement.Move(mxSpeed, jogSpd, _minMvSpd, CurVel, GndSpdMod, ref _prevMaxSpd);
        _baseMaxSpd = mxSpeed; 
    }


    /// <summary>
    /// Returns current speed after applying either Acceleration or Deceleration.
    /// </summary>
    /// <param name="moving"></param>
    /// <param name="curSpeed"></param>
    /// <param name="maxBaseSpd"></param>
    /// <returns></returns>
    private float GetCurSpdAccel(bool moving, float curSpeed)
    {        
        // If in air, limit speed.
        if (!gndCheck.Grounded)                            
            return .2f + (Mathf.Clamp(CurVel * .005f, 0, .015f));        
             
        float acclBonus = 0;
        // If base max spd < dynamic MaxSpd, then going down slope so increase max spd.
        if (_baseMaxSpd <= MaxSpd) 
             acclBonus += (CurVel + GndSpdMod) / 3; // Increase dynamic max speed (down slope).
        else acclBonus -= (CurVel + GndSpdMod) / 3; // Decrease it (up slope).
            
        // Accel if slower than max, decel if too fast.
        if (gndCheck.Grounded && moving && curSpeed < MaxSpd)
            curSpeed += (accelMod + acclBonus) * Time.deltaTime;
        else if(curSpeed > MaxSpd || !gndCheck.Grounded)
            curSpeed -= (accelMod + (acclBonus * 1.5f)) * Time.deltaTime;
        
        // Round down to max speed if close enough.
        if (Mathf.Abs(curSpeed - MaxSpd) < .2 && curSpeed > MaxSpd)
            curSpeed = MaxSpd;
        else if (curSpeed < 0)
            curSpeed = 0;

        // Update applied current speed.
        return curSpeed;
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
      //  Debug.Log(gndCheck.Grounded);
    }


    /// <summary>
    /// If not on cooldown, add jump force to vertical move velocity & give small dist boost.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="cooldown"></param>
    protected void Jump()
    {       
        if (!CanJump || !gndCheck.Grounded) return;
        moveVel.y += jumpAmount;
    }

    
    /// <summary>
    /// Set can't jump until cooldown time end.
    /// </summary>
    /// <param name="cooldown"></param>
    /// <returns></returns>
    private IEnumerator JumpCooldown(float cooldown)
    {
        if (!StartedJmpCd) // Cooldown once.
        {
            StartedJmpCd = true;
            yield return new WaitForSeconds(cooldown);
            CanJump = true;
            StartedJmpCd = false;
        }       
    }


    /// <summary>
    /// Move humanoid character using move velocity.
    /// </summary>
    private void ApplyMoveVelocity()
    {
        if(MaxSpd == 0)
        {
            moveVel.x = 0;
            moveVel.z = 0;
        }
        controller.Move(moveVel);
    }
}
