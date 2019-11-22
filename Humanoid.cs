using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : MonoBehaviour
{
    protected GroundCheck gndCheck; // If grounded, incline checks, etc.
    protected CharacterController controller; // Movement handling.
    protected float Gravity { get; set; } = .04f;
 
    private bool startedJmpCd = false; // Only allow one active jump cooldown.
    private bool canJump = true; // Jump cooldown lock.

    private float curSpd, maxSpd;

    public float jogSpd = 2, runSpd = 4;
    public float jumpAmount = 2, jumpCooldown = .4f;


    void Awake()
    {
        gndCheck = GetComponent<GroundCheck>(); 
        controller = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    protected void Update()
    {      
        if (gndCheck.Grounded && !canJump) // Start jump cooldown once grounded.
            StartCoroutine(JumpCooldown(jumpCooldown));

        GravityCalc(Gravity);
        ApplyMoveVelocity();

        if (gndCheck.Grounded) // If grounded, limit move speed.
        {
            moveVel.x = 0;
            moveVel.z = 0;
        }    
        else canJump = false; // Can't jump if not grounded. 
        Debug.Log(controller.velocity.magnitude);
    }


    /// <summary>
    /// Rotates humanoid to look at target position.
    /// </summary>
    /// <param name="newDir"></param>
    /// <param name="dampening"></param>
    /// <param name="rotSpeed"></param>
    protected void Rotate(Vector3 newDir, int dampening, int rotSpeed)
    {
        Vector3 curMoveDir = Vector3.Lerp(transform.position, newDir, dampening);
        // Lerp from cur rot to new one.
        transform.rotation = (Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(new Vector3(curMoveDir.x, 0, curMoveDir.z)), rotSpeed * Time.deltaTime));
    }

    private Vector3 moveVel = Vector3.zero;
    /// <summary>
    /// Apply move speed to horizontal move velocity.
    /// </summary>
    /// <param name="speed"></param>
    protected void Move(float speed)
    {
        float curVel = .5f * Vector3.Magnitude(new Vector3(controller.velocity.x, 0, controller.velocity.z));
        float gndSpdMod = .5f * (gndCheck.GroundSlope * .4f);
            
        // Max speed more/less depending on terrain incline.
        if (controller.velocity.y >= 0)
            maxSpd = (speed + curVel) - (gndSpdMod * .15f);  // Less speed if go up incline.
        else maxSpd = (speed + curVel) + (gndSpdMod * .15f); // More speed if down incline.

        // Limit min/top max speed.
        if (maxSpd < 0) maxSpd = 0;
        else if (maxSpd > speed * 2) maxSpd = speed * 2;

        if (gndCheck.Grounded)
            moveVel += (transform.forward * maxSpd) * Time.deltaTime;        
    }

    private float MoveAccel(bool moving)
    {
        // Cur velocity magnitude -> faster accel, greater incline -> slower accel
        curSpd = ( ((curSpd * .25f) + controller.velocity.magnitude) - (gndCheck.GroundSlope * .25f) ) * Time.deltaTime;
        
        // Limit max/min speed.
        if (curSpd > maxSpd) 
            curSpd = maxSpd;
        else if (curSpd < 0) 
            curSpd = 0;

        return curSpd;
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
    /// If not on cooldown, add jump force to vertical move velocity.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="cooldown"></param>
    protected void Jump()
    {       
        if (!canJump || !gndCheck.Grounded) return;
        moveVel.y += jumpAmount;
    }

    
    /// <summary>
    /// Set can't jump until cooldown time end.
    /// </summary>
    /// <param name="cooldown"></param>
    /// <returns></returns>
    private IEnumerator JumpCooldown(float cooldown)
    {
        if (!startedJmpCd) // Cooldown once.
        {
            startedJmpCd = true;
            yield return new WaitForSeconds(cooldown);
            canJump = true;
            startedJmpCd = false;
        }       
    }


    /// <summary>
    /// Move humanoid character using move velocity.
    /// </summary>
    private void ApplyMoveVelocity()
    {
        controller.Move(moveVel);
    }



}
