using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : MonoBehaviour
{
    protected GroundCheck gndCheck; // If grounded, incline checks, etc.
    protected CharacterController controller; // Movement handling.
    protected float Gravity { get; set; } = .032f;
 
    [HideInInspector] public bool StartedJmpCd { get; private set; } = false; // Only allow one active jump cooldown.
    [HideInInspector] public bool CanJump { get; private set; } = true; // Jump cooldown lock.
    [HideInInspector] public float CurSpd { get; private set; }
    [HideInInspector] public float MaxSpd { get; private set; }

    public float jogSpd = 1.45f, runSpd = 2f;
    public float jumpAmount = 0.35f, jumpCooldown = 0.8f;
    public float accelMod = 10; // Used to calculate cur speed.

    void Awake()
    {
        gndCheck = GetComponent<GroundCheck>(); 
        controller = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    protected void Update()
    {      
        SetCurSpeed(MaxSpd > 0, CurSpd, MaxSpd);
        GravityCalc(Gravity);

        if (gndCheck.Grounded && !CanJump) // Start jump cooldown once grounded.
            StartCoroutine(JumpCooldown(jumpCooldown));

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
    private float GndSpdMod => (.5f * (gndCheck.GroundSlope * .4f)) * .15f; // Terrain incline/decline speed modifier.
    private Vector3 moveVel = Vector3.zero;                                 // Direction to move.


    /// <summary>
    /// Apply move speed to horizontal move velocity.
    /// </summary>
    /// <param name="speed"></param>
    protected void Move(float speed)
    {    
        // Max speed more/less depending on terrain incline.
        if (controller.velocity.y >= 0)
            MaxSpd =  (speed + CurVel) - GndSpdMod; // Less top speed if go up incline.
        else MaxSpd = (speed + CurVel) + GndSpdMod; // More top speed if down incline.

        // Limit min/top max speed.     
        if(speed != 0)
        {
            if (MaxSpd < 0) MaxSpd = 0;
            else if (MaxSpd > speed * 1.6f) 
                MaxSpd = speed * 1.6f;
        }
       

        if (gndCheck.Grounded)
            moveVel += (transform.forward * CurSpd) * Time.deltaTime;        
    }


    /// <summary>
    /// Accel/Decel for current speed.
    /// </summary>
    /// <param name="moving"></param>
    /// <param name="curSpeed"></param>
    /// <param name="maxBaseSpd"></param>
    /// <returns></returns>
    private void SetCurSpeed(bool moving, float curSpeed, float maxBaseSpd)
    {
        float acclBonus = 0;
        if (maxBaseSpd <= MaxSpd) // Down slope - accel faster.
            acclBonus += (CurVel + GndSpdMod) / 3;
        else acclBonus -= (CurVel +  GndSpdMod) / 3;
            
        if (moving)        
             curSpeed += (accelMod + acclBonus) * Time.deltaTime;
        else curSpeed -= (accelMod + acclBonus) * Time.deltaTime;
                
        // Limit min/max speed.
        if (curSpeed > MaxSpd)
            curSpeed = MaxSpd;
        else if (curSpeed < 0)
            curSpeed = 0;
        CurSpd = curSpeed;
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
