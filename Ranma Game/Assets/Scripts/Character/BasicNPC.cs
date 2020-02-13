using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPC : Character
{
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animManager = GetComponent<CharacterAnimManager>();
        groundedCheck = GetComponent<GroundedCheck>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
