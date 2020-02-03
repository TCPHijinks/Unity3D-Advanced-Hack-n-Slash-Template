using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{  
    public bool IsGrounded { get; private set; }
    public (bool isBounceSurface, Transform surfaceTransform) BounceSurfaceCheck { get; private set; }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") IsGrounded = true;
        if (collision.gameObject.tag == "Bounce" && collision.gameObject != gameObject) BounceSurfaceCheck = (true, collision.transform);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") IsGrounded = true;
        if (collision.gameObject.tag == "Bounce" && collision.gameObject != gameObject) BounceSurfaceCheck = (true, collision.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") IsGrounded = false;
        if (collision.gameObject.tag == "Bounce" && collision.gameObject != gameObject) BounceSurfaceCheck = (false, collision.transform);
    }
}

