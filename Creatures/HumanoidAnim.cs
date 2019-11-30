using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnim : MonoBehaviour
{   
    private Animator anim;
   
    // Start is called before the first frame update
    void Start()
    {       
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Returns true if specified Animator layer is in an "Idle State". Done by checking for the "Layer Idle State" behavour on the current animator anim state node.
    /// </summary>
    /// <param name="layerIndex"></param>
    /// <returns>If in idle animation state that contains "Layer Idle State" behaviour.</returns>
    public bool LayerStateIdle(int layerIndex)
    {
        try
        {
            StateMachineBehaviour[] b = anim.GetBehaviours(anim.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash, layerIndex);
            LayerIdleState idleBehavour = (LayerIdleState)b[0];
            return true; // Return true if succeeded in finding idle state.
        }
        catch
        {
            return false;// Failed, was no LayerIdleState behavo
        }
    }



    public void UpateHumanoidAnims(bool doJmpCd, bool grounded, bool block, bool inCombat,
        bool attkFin, float curSpd, int attkStage)
    {       
        // Jumping if can't jump and not on cooldown after landing.
        bool jumping = !grounded && !doJmpCd;
        anim.SetBool("Jump", jumping);
        anim.SetFloat("Speed", curSpd);
        anim.SetBool("OnGround", grounded);
        anim.SetBool("InCombat", inCombat);
        anim.SetInteger("Attack", attkStage);
        anim.SetBool("Block", block);
        anim.SetBool("AttkFin", attkFin);
    }  
}