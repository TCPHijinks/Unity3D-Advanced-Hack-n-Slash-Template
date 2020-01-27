using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Rigidbody rigidbody;
    // protected CharacterController cControl;
    [SerializeField][Range(0,1)] protected float moveSpeed = .1f;




    protected void MoveAndRotate(Vector2 moveDirRequest)
    {
        // No move/rotation if not moving.
        if (moveDirRequest == Vector2.zero)
        {
            moving = false;
            return;
        }
        moving = true;

        // Don't move/rotate if too slow.
        if (Mathf.Abs(moveDirRequest.sqrMagnitude) < deadZone) return;
        
       
        float absoluteX = Mathf.Abs(moveDirRequest.x);
        float absoluteY = Mathf.Abs(moveDirRequest.y);
        int roundedX = RoundToNonZero(moveDirRequest.x);
        int roundedY = RoundToNonZero(moveDirRequest.y);
               
        bool canDiagonal = absoluteX > diagonalDeadZone && absoluteY > diagonalDeadZone;
        
        Vector3 moveDir;
        if (canDiagonal)
        {
            moveDir = new Vector3(roundedX, 0, roundedY);
        }
        else if (absoluteX > absoluteY)
        {
            moveDir = new Vector3(roundedX, 0, 0);
        }
        else
        {
            moveDir = new Vector3(0, 0, roundedY);
        }

             
        ApplyRotation(moveDir);

        // Update speed for FixedUpdate rigidbody movement.
        speed = (Mathf.Abs(moveDirRequest.sqrMagnitude) * moveSpeed) * 500;
    }   
    private readonly float deadZone = .05f;
    [SerializeField] private float diagonalDeadZone = .2f;
    float speed;
    bool moving = false;


    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        if(moving) rigidbody.AddForce(transform.forward * speed);
    }
   

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
}
