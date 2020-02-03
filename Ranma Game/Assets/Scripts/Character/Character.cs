using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Rigidbody rb;
    protected CharacterAnimManager animManager;

    [HideInInspector] public GroundedCheck groundedCheck;

    [Header("Movement")]
    [Space(10)]
    [SerializeField] [Range(0, 1)]      protected float moveSpeed = .6f;
    [SerializeField] [Range(0, 9000)]   private int shortJumpDownForce = 1000;   
    [SerializeField] [Range(0, 900)]    private float jumpForce = 90f;
     
    protected void MoveAndRotate(Vector3 moveDir){
        if (!groundedCheck.IsGrounded || _knockback.doingRequest) return;
        var t = GetMoveAndRotate(moveDir);
        _speed = t.speed;
        ApplyRotation(t.rotationDir);
    }


    /// <summary>
    /// Move character and rotate to move direction.
    /// </summary>
    /// <param name="moveDirRequest"></param>
    private (Vector3 rotationDir, float speed) GetMoveAndRotate(Vector2 moveDirRequest)
    {
        // No moving or rotation during attacks or not moving.
        if (animManager.CanDoDamage || moveDirRequest == Vector2.zero)
        {
            _speed = 0;
            _moving = false;
            return (rotationDir:transform.forward, speed: 0);
        }
        _moving = !_knockback.startRequest;


        // Absolute x & y for comparison.
        float absoluteX = Mathf.Abs(moveDirRequest.x);
        float absoluteY = Mathf.Abs(moveDirRequest.y);

        // Rounded x & y for direction of movement.
        int roundedX = RoundToNonZero(moveDirRequest.x);
        int roundedY = RoundToNonZero(moveDirRequest.y);
                
        bool moveDiagonal = absoluteX > _deadZone && absoluteY > _deadZone;


        Vector3 moveDir;
        if (moveDiagonal)       
        {
            moveDir = new Vector3(roundedX, 0, roundedY);
        }
        // Only moving left or right.
        else if (absoluteX > absoluteY)
        {
            moveDir = new Vector3(roundedX, 0, 0);
        }
        // Only moving forward or back.
        else
        {
            moveDir = new Vector3(0, 0, roundedY);
        }

        return (rotationDir: moveDir, (Mathf.Abs(moveDirRequest.sqrMagnitude) * moveSpeed) * 500);     
    }    
    private readonly float _deadZone = .4f;
    private bool _moving = false;

    
   

    public void DoKnockback(float force, Transform src) => _knockback = (true, src, force, false);
    private (bool startRequest, Transform src, float force, bool doingRequest) _knockback = (false, null, 0, false);




    /// <summary>
    /// Update physics.
    /// </summary>
    private void FixedUpdate()
    {
        bool IsGrounded = groundedCheck.IsGrounded;
        if (groundedCheck.BounceSurfaceCheck.isBounceSurface)
        {
            Debug.Log("WORK");
            _knockback = (true, groundedCheck.BounceSurfaceCheck.surfaceTransform, jumpForce, true);
        }

        
        if(!_knockback.startRequest && _knockback.doingRequest )
        {
            float vel = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
            if (vel < 1) _knockback.doingRequest = false;
        }

        // Reset horizontal velocity and move if able.
        if (_moving && !_knockback.startRequest && IsGrounded && !_knockback.doingRequest)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.AddForce(transform.forward * (_speed * 2.5f));
        }
        // Fall faster if in air.
        else if (!_doJump)
        {           
            rb.AddForce(-transform.up * 500);
        }


        // If knockback requested, apply appropriate force.
        if (_knockback.startRequest)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.AddForce(CalcStunMoveDir(_knockback.src) * _knockback.force, ForceMode.Impulse);
            _knockback.startRequest = false;
        }
        // Apply jump force if can jump & is requested.
        else if (_doJump && IsGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        // Stop jump state if falling down.
        if (_doJump && rb.velocity.y < 0 && !IsGrounded)
        {         
            _doJump = false;
        }
        // Shorten jump by adding extra down force if not holding jump during ascent. 
        else if (!_doJump && rb.velocity.y > 0 && !IsGrounded && !_knockback.doingRequest)
        {          
            rb.AddForce(-transform.up * shortJumpDownForce);
        }
    }
    private float _speed;
    


    private Vector3 CalcStunMoveDir(Transform damageSource) => (transform.position - damageSource.position).normalized;
    
        

    

  



    /// <summary>
    /// Rotate character to look direction.
    /// </summary>
    /// <param name="lookDir"></param>
    private void ApplyRotation(Vector3 lookDir) => transform.rotation = Quaternion.LookRotation(lookDir);




    /// <summary>
    /// Returns next integer away from 0.
    /// </summary>
    /// <param name="toRound"></param>
    /// <returns></returns>
    private int RoundToNonZero(float toRound)
    {      
        if (toRound == 0) return 0;
        return toRound > 0 ? 1 : -1; 
    }

    

    protected void Maneuver()
    {        
        _doJump = true;      
    }
    private bool _doJump = false;

    protected void ManeuverEnd()
    {      
        _doJump = false;
    }

    protected void AttackStd()
    {
        _moving = false;
        animManager.DoAttack(AttkType.standard);
    }

    protected void AttackHeavy()
    {
        _moving = false;
        animManager.DoAttack(AttkType.heavy);       
    }

    protected void Interact()
    {
        Debug.Log("JUST USE IT!");
    }
}
