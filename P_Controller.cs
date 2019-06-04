using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P_Controller : MonoBehaviour
{
    // Characteristics
    private readonly float mass = 20.4f;
   
    // Misc.
    [SerializeField] Camera cam;            // Camera movement dir is relative to.
    [SerializeField] Transform groundCheck; // Transform of feet to know if grounded.
    [SerializeField] LayerMask ground;      // Ground layer mask.   
    [SerializeField] Vector3 Drag;
    
    // Mobility action variables.
    [SerializeField] [Range(0, 10f)] private float jumpHeight = 2f;
    [SerializeField] [Range(0, 03f)] private float jumpCooldown = 1.5f;
    [SerializeField] [Range(0, 03f)] private float groundDistance = 1f;     // Player detect ground to jump.
    [SerializeField] [Range(0, 10f)] private float dashDistance = 5f;

    // Falling and Physics variables.
    [SerializeField] [Range(0, 05f)] private float fallMultiplier = 1.5f;   // Amount multiply gravity when character falling.
    [SerializeField] [Range(0, 05f)] private float lowJumpMultiplier = 1f;  // Amount multiply gravity when do little jump.
    [SerializeField] [Range(00, 50)] private int staggerThreshold = 12;     // How long player can fall before staggering.

    // General movement variables.
    [SerializeField] [Range(0, 10f)] private float rotationSpeed = 9.4f;    // Speed of the player rotation.
    [SerializeField] [Range(1, 10f)] private float accelModifier = 9.2f;    // Acceleration modifer for movement.    
    [SerializeField] [Range(0, 10f)] private float jogSpeed = 17.5f;        // Player default base move speed.    
    [SerializeField] [Range(0, 10f)] private float runSpeed = 27.8f;        // Player default base run speed.
    [SerializeField] [Range(0, 10f)] private float crouchSpeed = 10.6f;     // Player default base crouch move speed.

    [SerializeField] private float slopeForceRayLength = 2f;    // Multiplier of down ray used to determine whether on a slope.
    
    CharacterController cController;    
    Vector3 moveDir;    // Direction of movement.
    Vector3 velocity;   // Forces that affect/impose movement.

    public bool grounded = true;    // Whether grounded to jump.  **Public for Debug*
    public bool canJump = true;  
    public float gravity = -9.8f;  // Gravity that affects player.
    private float baseSpeed;        // Base speed of movement state.
    private float curSpeed;         // Current movement speed.
    private float _accelModifier;



    // Start is called before the first frame update
    void Start()
    {
        cController = GetComponent<CharacterController>();
    }



    // Update is called once per frame
    void Update()
    {
        // Check if player grounded.
        grounded = Physics.CheckSphere(groundCheck.position,
            groundDistance, ground, QueryTriggerInteraction.Ignore);
        
        // Rotate to move direction unless not moving.
        if (moveDir.x != 0 && moveDir.z != 0)
            transform.rotation = (Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.z)), rotationSpeed * Time.deltaTime));
        
        // Update if walking and direction.
        moveDir = MoveInputDir();

        cController.Move(moveDir * Time.deltaTime * curSpeed);
        
        // Gravity and Mobility Movements.
        MobilitySkillCheck();   // Check for and update velocity if char uses a mobility skill (e.g. jumping).
        Jump();                 //  Check if jump, apply addittional forces depending on type of jump.
        StaggeringFall();
        SetPhysicsVelocity();   // Update gravity & drags effects on character velocity.    
        
    
        // Apply calculated velocity.
        cController.Move(velocity * Time.deltaTime);
    }



    // Movement speed with acceleration calculation.
    private float AcceleratedSpeed()
    {
        // Calculate rate of acceleration.
        float acceleration = baseSpeed / mass;

        // Accelerate if moving until max base speed.
        if (curSpeed < baseSpeed)
            return curSpeed + (0.8f * acceleration) * (_accelModifier * 2.5f) * Time.deltaTime;
        else
            return baseSpeed;
    }



    // Determine move state and speed.
    private void UpdateMoveSpeed()
    {
        // Only update move state speed if grounded.
        if (grounded)
            // Set cur speed if not moving.
            if (moveDir.x == 0 && moveDir.z == 0)
                curSpeed = 0;
            else
                curSpeed = AcceleratedSpeed();

        // Movement state check.
        if (Input.GetKey(KeyCode.LeftControl))
            baseSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            baseSpeed = runSpeed;
        else
            baseSpeed = jogSpeed;

        // If just staggered, set speed to lowest base speed,
        //  and also slow acceleration on take off.        
        if (StaggeringFall())
        {
            _accelModifier = accelModifier / 2;
            baseSpeed = crouchSpeed / 2;
        }

        // If not staggered and reached speed threshold, return 
       //   active acceleration to normal base value.        
        if (curSpeed > crouchSpeed && !StaggeringFall())
            _accelModifier = accelModifier;
    }



    // Updates movement walk/run dir.
    private Vector3 MoveInputDir()
    {
        // Get and set forward & right relative to camera.
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        Vector3 inputDir;

        // Stop cam from making char lean forward.
        forward.y = 0f;
        right.y = 0f;

        // Only read directional input if grounded.
        if (grounded)
        {
            // Update movement speed based on input.
            UpdateMoveSpeed();

            // Get & clamp move input, set move dir relative to camera position. 
            inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // Get player move input.
            inputDir = Vector3.ClampMagnitude(inputDir, 1f);  // Clamp input magnitude to prevent diagnal move bug.

            // Prevent diagnal move speed bug.
            forward.Normalize();
            right.Normalize();

            // Set move dir based input dir relative to where the camera is looking.                 
            return (forward * inputDir.z + right * inputDir.x);
        }
        // Return no input if not grounded.
        return moveDir;
    }



    // Returns velocity calculated from gravity and drag, 
    //  also keeps player from bouncing on slopes.
    private void SetPhysicsVelocity()
    {
        // Apply gravity.
        velocity.y = velocity.y - gravity * Time.deltaTime;
      
        // Apply drag to make character stop moving,
        //  also prevents velocity NaN errors.
        velocity.x /= 1 + Drag.x * Time.deltaTime;
        velocity.y /= 1 + Drag.y * Time.deltaTime;
        velocity.z /= 1 + Drag.z * Time.deltaTime;

        // Extra force to keep player from bouncing when descending slopes.
        if (OnSlope() && GetVelocityY() <= 0)        
            velocity.y -= (gravity * 4 * Time.deltaTime);
    }



    // Handles checks for active movement abilities without much complexity.
    private void MobilitySkillCheck()
    {
        // Air tuck and slight dash.
        if (Input.GetKeyDown(KeyCode.LeftControl) && !grounded)
        {
            Debug.Log("Player has Dashed & Tucked Legs in-Air!");
            velocity += Vector3.Scale(transform.forward, 
                dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, 
                (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
        }
    }



    // How Work: When falling the player is accelerated to quicken all falls. Holding the
    //  "Jump" button will increase how long the player can be in the air.
    //
    //  1. Pressing the "Jump" button will perform a jump if possible.
    //  2. If in the air and 'Gaining Alitude' (jumping) while 'Not Holding Down' the
    //      jump key. An extra gravitation force will shorten the time gaining altitude.
    //  3. Else, if in the air and 'Losing Altitude'. An extra downward force is applied
    //      to quicken the fall. Making it less floaty.        
    private void Jump()
    {
        // Jump only if grounded.
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            if (canJump)
            {
                velocity.y += (jumpHeight * 3 * gravity);
                StartCoroutine(JumpCooldown());
            }
        }
        if (!grounded)
        {
            // If getting higher and not holding Jump. Do a *short jump*.       
            if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                // Apply (weak) additional gravity to y velocity.
                velocity.y -= (gravity * mass) * (lowJumpMultiplier * 2.5f) * Time.deltaTime;              
            }

            // If falling, apply extra gravity to make the fall much faster.        
            else if (velocity.y < 0 && !grounded && !OnSlope())
            {
                // Apply additional (strong) gravity to y velocity every second.
                velocity.y -= (gravity * mass) * (fallMultiplier * 1.5f) * Time.deltaTime;
            }
        }
    }



    // Prevent jumping until cooldown end.
    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    

    // Returns whether the player is on a sloped surface.
    bool OnSlope()
    {
        // Not on slope if in air.
        if (!grounded)
            return false;

        RaycastHit hit; // Store hit information of ray.

        // Cast raycast downwards at half the char height + multiplier from its center origin.
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, slopeForceRayLength, ground))
            if (hit.normal != Vector3.up)
                return true;

        // Not on sloped surface.
        return false;
    }



    // Calculates the velocity of the y Axis for the purpose of knowing
    //  whether the player is falling or climbing.
    Vector3 prevPos, curPos;    
    float GetVelocityY()
    {       
        curPos = transform.position;

        float yVelocity = (curPos.y - prevPos.y) / Time.deltaTime;

        if (curPos != prevPos)
            prevPos = curPos;

        return yVelocity;
    }



    // Stops player momentum/staggers them when they fall for X duration.
    public int fallDur = 0; // Counts how long player has been falling.    
    private bool StaggeringFall()
    {
        // COntinue to count up whilst in air.
        if (!grounded && fallDur > 0)        
            fallDur++;        

        // Start counting when first start falling.
        if (!grounded && GetVelocityY() < 0 && fallDur == 0)        
            fallDur++;   
       

        // Check if landed.
        if (grounded)
        {
            // Check if a large fall.
            if (fallDur >= staggerThreshold)
            {
                fallDur = 0;  // Reset counter for next fall.
                return true;    // Return that it was a big fall.
            }
          
            fallDur = 0;  // Reset counter for next fall.
            return false;   // Return that it was Not a big fall.
        }

        return false;   // Return false because still falling.
    }
}

