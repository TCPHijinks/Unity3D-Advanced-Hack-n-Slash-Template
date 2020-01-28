using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Rigidbody rb;
    protected CharacterAnimManager animManager;
    [SerializeField][Range(0,1)] protected float moveSpeed = .1f;



    /// <summary>
    /// Move character and rotate to move direction.
    /// </summary>
    /// <param name="moveDirRequest"></param>
    protected void MoveAndRotate(Vector2 moveDirRequest)
    {
        // No moving or rotation during attacks.
        if (animManager.CanDoDamage)
        {
            speed = 0;
            return;
        }

        // No move/rotation if not moving.
        if (moveDirRequest == Vector2.zero)
        {
            moving = false;
            return;
        }
        moving = true;
        
        
        // Absolute x & y for comparison.
        float absoluteX = Mathf.Abs(moveDirRequest.x);
        float absoluteY = Mathf.Abs(moveDirRequest.y);
        // Rounded x & y for direction of movement.
        int roundedX = RoundToNonZero(moveDirRequest.x);
        int roundedY = RoundToNonZero(moveDirRequest.y);
                
        bool moveDiagonal = absoluteX > deadZone && absoluteY > deadZone;


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

        // Rotate to direction moving.     
        ApplyRotation(moveDir);

        // Update speed for FixedUpdate physics movement method.
        speed = (Mathf.Abs(moveDirRequest.sqrMagnitude) * moveSpeed) * 500;
    }    
    private readonly float deadZone = .4f;
    private bool moving = false;
    


    /// <summary>
    /// Update physics.
    /// </summary>
    private void FixedUpdate()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if(moving) rb.AddForce(transform.forward * speed);
    }
    private float speed;



    /// <summary>
    /// Rotate character to look direction.
    /// </summary>
    /// <param name="lookDir"></param>
    private void ApplyRotation(Vector3 lookDir)
    {
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
       


    /// <summary>
    /// Returns next integer away from 0.
    /// </summary>
    /// <param name="toRound"></param>
    /// <returns></returns>
    private int RoundToNonZero(float toRound)
    {
        if (toRound > 0) return Mathf.CeilToInt(toRound);
        else return Mathf.FloorToInt(toRound);
    }

    protected void Maneuver()
    {
        Debug.Log("JUMP! ROLL!");
    }

    protected void AttackStd()
    {
        animManager.DoAttack(AttkType.standard);
    }

    protected void AttackHeavy()
    {
        Debug.Log("KICK TO THE DICK");
    }

    protected void Interact()
    {
        Debug.Log("JUST USE IT!");
    }
}
