using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundCheck : MonoBehaviour
{
    
    CharacterController controller;

    [HideInInspector] public bool Grounded => g.solver.rootHit.distance != 0 && g.solver.rootHit.distance < 1;

    [HideInInspector] public Vector3 HitNormal { get; private set; }    // Normal of ground surface.

    [HideInInspector] public float GroundSlope => Vector3.Angle(Vector3.up, HitNormal);

    void OnControllerColliderHit(ControllerColliderHit hit) => HitNormal = hit.normal;

    GrounderFBBIK g;
    void Start()
    {
       g = GetComponent<GrounderFBBIK>();
       // Debug.Log(g.solver.rootGrounded);
        controller = GetComponent<CharacterController>();

    }
   
  //  public float hOffset = .3f, length = 10f;
 //   public bool AmGrounded { 
 //       get 
    //    {
 //           
   //         Debug.DrawRay((new Vector3(transform.position.x, transform.position.y + hOffset, transform.position.z)), Vector3.down * length, Color.green, .1f);
  //          return (Physics.Raycast((new Vector2(transform.position.x, transform.position.y + hOffset)), Vector3.down, length, 1 << LayerMask.NameToLayer("Ground"))); // raycast down to look for ground is not detecting ground? only works if allowing jump when grounded = false; // return "Ground" layer as layer

  //      } 
  //  }
}
