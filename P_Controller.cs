using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P_Controller : MonoBehaviour
{
    // Characteristics
    private readonly float mass = 20.4f;
    [SerializeField] Camera cam;            // Camera movement dir is relative to.
    [SerializeField] Transform groundCheck; // Transform of feet to know if grounded.
    [SerializeField] LayerMask ground;      // Ground layer mask.   
    [SerializeField] Vector3 Drag = new Vector3(4, 5, 4);
        
    [Header("Mobility action variables.")]
    [Space(20)] // 10 pixels of spacing here.
    [SerializeField] [Range(0, 10f)] private float jumpHeight = 0.51f;
    [SerializeField] [Range(0, 03f)] private float jumpCooldown = 0.72f;
    [SerializeField] [Range(0, 03f)] private float groundDistance = 0.26f;     // Player detect ground to jump.
    [SerializeField] [Range(0, 10f)] private float dashDistance = 5f;
    [SerializeField] [Range(0, 10f)] private float slideSpeedBonus = 3f;
    [SerializeField] [Range(0, 99f)] private float slideDeceleration = 5f;

    [Header("Falling and Physics variables.")]
    [Space(20)] // 10 pixels of spacing here.
    [SerializeField] [Range(0, 05f)] private float fallMultiplier = 0.87f;   // Amount multiply gravity when character falling.
    [SerializeField] [Range(0, 05f)] private float lowJumpMultiplier = 0.6f;  // Amount multiply gravity when do little jump.
    [SerializeField] [Range(00, 99)] private int staggerThreshold = 10;     // How long player can fall before staggering.
           
    [Header("General movement variables.")]
    [Space(30)] // 10 pixels of spacing here.
    
    [SerializeField] [Range(0, 10f)] private float rotationSpeedCrouching = 9.5f;   // Speed of the player rotation.   
    [SerializeField] [Range(0, 10f)] private float rotationSpeedJogging = 6.95f;     // Speed of the player rotation.   
    [SerializeField] [Range(0, 10f)] private float rotationSpeedRunning = 6.65f;     // Speed of the player rotation.   
    [SerializeField] [Range(0, 99f)] private float dirDampenCrouch = 3f;    // Base dampening value applied when crouching.
    [SerializeField] [Range(0, 99f)] private float dirDampenJog = 15f;      // Base dampening value applied when jogging.
    [SerializeField] [Range(0, 99f)] private float dirDampenRun = 87.7f;    // Base dampening value applied when running.
    [SerializeField] [Range(1, 10f)] private float accelModifier = 9.35f;   // Acceleration amount modifier applies to movement.    
    [SerializeField] [Range(1, 10f)] private float decelModifier = 9.35f;   // Deceleration amount modifier applies to movement.  
    [SerializeField] [Range(0, 10f)] private float jogSpeed = 3.86f;        // Player default base move speed.    
    [SerializeField] [Range(0, 10f)] private float runSpeed = 5.99f;        // Player default base run speed.
    [SerializeField] [Range(0, 10f)] private float crouchSpeed = 1.55f;     // Player default base crouch move speed.
    [SerializeField] [Range(0, 10f)] private float slopeForceRayLength = 2f;// Multiplier of down ray used to determine whether on a slope.
    
    CharacterController cController;    
    Vector3 moveDir;    // Direction of movement.
    Vector3 velocity;   // Forces that affect/impose movement.

    private bool grounded = true;    // Whether grounded to jump.  **Public for Debug*
    private bool canJump = true;     // Whether the player is capable of jumping.    
    private float baseSpeed;        // Base speed of movement state.
    private float curSpeed;         // Current movement speed.
    private float _accelModifier;   // Dynamic modifer applied to movement acceleration.
    private float _decelModifier;   // Dynamic modifier applied to movement deceleration.  
    private float _slideDeceleration;
    private bool jumped = false;        // Whether player has just jumped.
    private bool allowDirInput = true;  // Whether controller will read player movement input.
    private bool slideLock = false;     // Whether player is currently locked in a slide.
    public float gravity = 3.2f;        // Gravity that affects player.
    public float dirChangeDampening;    // Dampening applied during dir changes.
    public float rotationSpeed;         // Speed of the player rotation.   





    // Start is called before the first frame update
    void Start()
    {
        cController = GetComponent<CharacterController>();
        _accelModifier = accelModifier;
        _decelModifier = decelModifier;
        _slideDeceleration = slideDeceleration;
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

       // Update movement speed and responsiveness.
        UpdateMoveSpeed();      // Update movement speed based on input and whether staggered.       
        UpdateMoveDampening();  // Update move responsiveness (dampening) based on move speed state.
        UpdateRotationSpeed();  // Update speed player rotates to move dir based on move speed state.

        // Update if walking and direction with dampening.
        moveDir = Vector3.Lerp(moveDir, MoveInputDir(), dirChangeDampening);
        cController.Move(moveDir * Time.deltaTime * curSpeed);
        
        // Gravity and Mobility Movements.
        MobilitySkillCheck();   // Check for and update velocity if char uses a mobility skill (e.g. jumping).
        Jump();                 //  Check if jump, apply addittional forces depending on type of jump.
        Slide();
        StaggeringFall();
        SetPhysicsVelocity();   // Update gravity & drags effects on character velocity.    
    
        // Apply calculated velocity.
        cController.Move(velocity * Time.deltaTime);
    }





    // Returns whether the player is on a sloped surface.
    RaycastHit hit; // Store hit information of ray.
    bool OnSlope()
    {
        // Not on slope if in air.
        if (!grounded)
            return false;
        
        // Cast raycast downwards at half the char height + multiplier from its center origin.
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, slopeForceRayLength, ground))
            if (hit.normal != Vector3.up)   // Check if ray doesn't bounce straight up (on slope).           
                return true;

        // Not on sloped surface.
        return false;
    }





    // Movement speed with acceleration calculation.
    private float AccelerateSpeed()
    {
        // Calculate rate of acceleration/deceleration.
        float acceleration = baseSpeed / mass;

        // Accelerate if moving until max base speed.
        if (curSpeed < baseSpeed)
            return curSpeed + (0.8f * acceleration) * (_accelModifier * 2.5f) * Time.deltaTime; // Accel if cur speed is less than intended speed.
        else
            return curSpeed - (0.8f * acceleration) * (_decelModifier * 2.5f) * Time.deltaTime; // Decel if cur speed is greater than intended speed.
    }




   
    // Determine move state and speed.  
    private void UpdateMoveSpeed()
    {
        // Only update move state speed if grounded.
        if (grounded)
        {
            // Set cur speed if not moving.
            if (moveDir.x == 0 && moveDir.z == 0)
                curSpeed = 0;
            else
                curSpeed = AccelerateSpeed();

            // Movement state check if not sliding.
            if(allowDirInput)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    baseSpeed = crouchSpeed;
                else if (Input.GetKey(KeyCode.LeftShift))
                    baseSpeed = runSpeed;
                else
                    baseSpeed = jogSpeed;
            }
        }     
        // If just staggered, set speed to lowest base speed,
        //  and also slow acceleration on take off.        
        if (StaggeringFall())
        {
            _accelModifier = accelModifier / 2;
            baseSpeed = crouchSpeed / 2;
        }
        // If not staggered and reached speed threshold, return 
        //  active acceleration to normal base value.        
        if (curSpeed > crouchSpeed && !StaggeringFall())
            _accelModifier = accelModifier;
    }




       
    private Vector3 MoveInputDir()
    {
        // Get and set forward & right relative to camera.
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        // Stop cam from making char lean forward.
        forward.y = 0f;
        right.y = 0f;

        // Get & clamp move input, set move dir relative to camera position. 
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // Get player move input.
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);  // Clamp input magnitude to prevent diagnal move bug.

        // Prevent diagnal move speed bug.
        forward.Normalize();
        right.Normalize();
        
        // Only read directional input if grounded.
        if (grounded && allowDirInput)
        {
            // Set move dir based input dir relative to where the camera is looking.                 
            return (forward * (inputDir.z) + right * (inputDir.x));
        }
        // Return no input if not grounded.
        return moveDir;
    }




    
    public GameObject bodyTEMP;   
    void Slide()
    {
        // Sliding. If can slide and press button, lock player into a slide.           
        if (curSpeed >= runSpeed && Input.GetKey(KeyCode.LeftAlt) && !slideLock)
        {
            Debug.Log("WWWWWWW");
            curSpeed += slideSpeedBonus;
            slideLock = true;
            allowDirInput = false;
        }

        // Unlock from slide when at crouch speed.
        if (curSpeed <= crouchSpeed && slideLock)
        {
            slideLock = false;
            allowDirInput = true;
        }

        // Check if player slide off edge and fall.
        if(!grounded && slideLock)
        {
            slideLock = false;
            allowDirInput = true;
            curSpeed = curSpeed / 2;
            fallDur = staggerThreshold; // Set staggery fall conditions true.
            StaggeringFall();   // Stagger player.
        }
        
        Debug.Log(slideLock);
        
        bodyTEMP.GetComponent<MeshRenderer>().material.color = Color.white;
        if (slideLock)
        {
            curSpeed -= slideDeceleration * Time.deltaTime;
            bodyTEMP.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }





    // Changes rotation speed and movement dir change dampening to match
    //  the speed of the character.
    private void UpdateMoveDampening()
    {
        // Update dir change dampening only on ground.
        if(grounded)
        {
            if (baseSpeed == crouchSpeed)
            {
                dirChangeDampening = dirDampenCrouch;
            }
                
            else if (baseSpeed == jogSpeed)
                dirChangeDampening = dirDampenJog;
            else
                dirChangeDampening = dirDampenRun;
        }
    }





    // Changes speed of rotation based off of move speed state.
    private void UpdateRotationSpeed()
    {
        // Update rotation speed if grounded.
        if(grounded)
        {
            // Assign rotation speed relative to move speed state.
            if (baseSpeed == crouchSpeed)
                rotationSpeed = rotationSpeedCrouching;
            else if (baseSpeed == jogSpeed)
                rotationSpeed = rotationSpeedJogging;
            else
                rotationSpeed = rotationSpeedRunning;
        }       
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
        if (OnSlope() && GetVelocityY() <= 0 && !jumped)        
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
        // Has no llonger just jumped if on ground.
        if (!canJump && !grounded)
            jumped = false;

        // Jump only if grounded.
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            if (canJump)
            {
                jumped = true;  // Has just jumped to allow jumping on slopes.
                velocity.y += (jumpHeight * 3 * gravity);   // Apply jumping force.
                StartCoroutine(JumpCooldown()); // Start jump cooldown.
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
       
    // Calculates the velocity of the y Axis for the purpose of knowing
    //  whether the player is falling or climbing.
    Vector3 curPos;    
    float GetVelocityY()
    {      
        // Save last position and get new current to calculate distance.
        Vector3 prevPos = curPos;       // Update previous pos to the last know position.
        curPos = transform.position;    // Update current position to new current position.
                
        // Return velocity of y axis.
        return (curPos.y - prevPos.y) / Time.deltaTime;
    }



    // Stops player momentum/staggers them when they fall for X duration.
    private int fallDur = 0; // Counts how long player has been falling.    
    private bool StaggeringFall()
    {
        // Only count fall dur when falling.
        if(GetVelocityY() < 0)
        {
            // COntinue to count up whilst in air.
            if (!grounded && fallDur > 0)
                fallDur++;

            // Start counting when first start falling.
            if (!grounded && fallDur == 0)
                fallDur++;
        }       
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

