using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    CharacterController controller;

    [HideInInspector] public bool Grounded => controller.isGrounded;

    [HideInInspector] public Vector3 HitNormal { get; private set; }    // Normal of ground surface.

    [HideInInspector] public float GroundSlope => Vector3.Angle(Vector3.up, HitNormal);

    void OnControllerColliderHit(ControllerColliderHit hit) => HitNormal = hit.normal;


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
}
