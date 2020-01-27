using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected CharacterController cControl;
    [SerializeField] protected float moveSpeed = 0;



   
    protected void MoveAndRotate(Vector2 moveDirRequest)
    {       
        // No move/rotation if not moving.
        if (moveDirRequest == Vector2.zero) return;
               
        // Don't move/rotate if too slow.
        if (Mathf.Abs(moveDirRequest.sqrMagnitude) < deadZone) return;
        
        // No diagonal movement. Move horizontal OR vertical.
        if (Mathf.Abs(moveDirRequest.x) > Mathf.Abs(moveDirRequest.y))
        {
            moveDirRequest.y = 0;
            ApplyRotation(new Vector3(RoundToNonZero(moveDirRequest.x), 0, 0));
        }
        else
        {
            moveDirRequest.x = 0;
            ApplyRotation(new Vector3(0, 0, RoundToNonZero(moveDirRequest.y)));
        }

        Vector3 moveDir = new Vector3(moveDirRequest.x * moveSpeed, 0, moveDirRequest.y * moveSpeed);
        moveDir = Vector3.Lerp(transform.position, moveDir, 1);
        cControl.Move(moveDir);       
    }
    private readonly float deadZone = .05f;





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
