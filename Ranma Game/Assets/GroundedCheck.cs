using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{  
    public bool IsGrounded { get; private set; }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") IsGrounded = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") IsGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") IsGrounded = false;
    }
}

