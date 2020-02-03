using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Rigidbody rb;
    protected CharacterAnimManager animManager;
    public GroundedCheck groundedCheck;
    [SerializeField] [Range(0, 1)] protected float moveSpeed = .1f;


    protected void MoveAndRotate(Vector3 moveDir){
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
        _moving = !_knockback.doRequest;


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

    
   

    public void DoKnockback(float force, Transform src) => _knockback = (doRequest: true, src, force);
    private (bool doRequest, Transform src, float force) _knockback = (false, null, 0);

    /// <summary>
    /// Update physics.
    /// </summary>
    private void FixedUpdate()
    {

        if (_moving && !_knockback.doRequest && groundedCheck.IsGrounded)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.AddForce(transform.forward * (_speed * 1.5f));
        }
        else if(!groundedCheck.IsGrounded)
        {
            rb.AddForce(-transform.up * 500);
        }

     //   

        if (_knockback.doRequest)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.AddForce(CalcStunMoveDir(_knockback.src) * _knockback.force, ForceMode.Impulse);
            _knockback.doRequest = false;
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
        Debug.Log("JUMP! ROLL!");
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
