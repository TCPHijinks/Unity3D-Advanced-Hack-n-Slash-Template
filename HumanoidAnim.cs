using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnim : MonoBehaviour
{
    private GrounderFBBIK grounderIK;
    private GroundCheck gndCheck;
    private Humanoid humanoid;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        grounderIK = GetComponent<GrounderFBBIK>();
        humanoid = GetComponent<Humanoid>();
        gndCheck = GetComponent<GroundCheck>();
        anim = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Jumping if can't jump and not on cooldown after landing.
        bool jumping = !humanoid.CanJump && !humanoid.StartedJmpCd;

        if (jumping) // Allow feet to leave ground when jump.
            grounderIK.weight -= (float)(2.5 * Time.deltaTime);        
        else        
            grounderIK.weight += (float) (2.5 * Time.deltaTime);
        
        anim.SetBool("Jump", jumping); 
        anim.SetFloat("Speed", humanoid.CurSpd);
        anim.SetBool("OnGround", gndCheck.Grounded);
    }
}
