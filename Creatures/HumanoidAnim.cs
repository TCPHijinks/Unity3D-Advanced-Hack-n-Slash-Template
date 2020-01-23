using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnim : CreatureAnimManager
{    
    
    /// <summary>
    /// Whether doing a roll animation and not in a transition.
    /// </summary>
    public bool DoingCombatRoll { get; private set; }

    private int baseLayer;  // Base layer hash.          
    private bool inBaseAnimTransition = false; // If base layer is transitioning between animations.
    private bool inRollState = false;   // If base layer is in roll animation state.
    private bool inStunState = false;   // If base layer is in stun animation state.  
   



    
    void Start()
    {
        baseLayer = anim.GetLayerIndex("Base Layer");   
    }







    [SerializeField] float cmbtMoveAnimDmp = .045f;

    /// <summary>
    /// Updates humanoid animator. Should be always called in Update().
    /// </summary>
    /// <param name="grounded"></param>
    /// <param name="block"></param>
    /// <param name="inCombat"></param>
    /// <param name="curSpd"></param>
    /// <param name="running"></param>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    /// <param name="attk"></param>
    /// <param name="doCmbtRoll"></param>
    /// <param name="rollDir"></param>
    /// <param name="crouching"></param>
    public void UpateAnimator(
        bool grounded, bool block, bool inCombat,
        float curSpd, bool running, float horizontal, float vertical, 
        AttkType attk, bool doCmbtRoll, int rollDir, bool crouching)
    {
        if(curSpd == 0)
        {
            horizontal = 0;
            vertical = 0;
        }

        inBaseAnimTransition = anim.IsInTransition(baseLayer);
    

        // Get & update combat rolling animation status.
        inRollState = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Roll");
        DoingCombatRoll = inRollState && !inBaseAnimTransition;
               
        // Get & update attacking animation status.
        InAttkState = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Attack") ||
            anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Shield Attack");
        DoingAttk = InAttkState && !inBaseAnimTransition;

        // Set stunned state & get stunned animation status. 
        anim.SetInteger("StunLevel(0-3)", StunLevel);
        inStunState = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Stunned");
        IsStunned = inStunState && !inBaseAnimTransition;
        
        // Get off-hand attack animation status.
        DoingOffhandAttack = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Shield Attack") && !inBaseAnimTransition;
      
        // Get rotation penalty from current animation.
        AttkRotationPenalty = anim.GetFloat("AttkRotationPenalty");
      
        // Update if in combat roll.
        CombatRollAnim(doCmbtRoll);
     

        // Set animator properties.
        anim.SetFloat("Speed", curSpd);
        anim.SetBool("OnGround", grounded);
        anim.SetBool("InCombat", inCombat);
        anim.SetBool("Crouching", crouching);
        anim.SetBool("Running", running);
        anim.SetFloat("Horizontal", horizontal, cmbtMoveAnimDmp, Time.deltaTime);
        anim.SetFloat("Vertical", vertical, cmbtMoveAnimDmp, Time.deltaTime);      
        anim.SetInteger("RollDir", rollDir);

        anim.SetBool("InAttkState", InAttkState);
        anim.SetBool("Block", block);

        // Set collider height & offset position to keep on ground.
        cc.height = anim.GetFloat("Collider_Scale") * defaultColliderHeight;
        if (cc.height == 0) cc.height = 1f;
        cc.center = new Vector3(cc.center.x, cc.height * .6f, cc.center.z);

        // Update attack queue with requested attack & handle attack animation logic.
        AttackAnim(attk);
               
        // Disable stun once animation playing.
        if(IsStunned || AttkDmgPercentage > 0)        
            StunLevel = 0;
    }

 
    


   
   
    private void AttackAnim(AttkType requestedAttk)
    {
        // Update que if valid request & not already transitioning to next anim.
        if (requestedAttk != AttkType.none && !inBaseAnimTransition)        
            quedAttk = requestedAttk;
       

        // Update que & cur attk if not doing attk or que request changes.
        if (curAttk == AttkType.none && requestedAttk != AttkType.none && requestedAttk != curAttk)
        {
            if (quedAttk != AttkType.none)
                newAttkQued = true;
            curAttk = quedAttk;
            quedAttk = AttkType.none;        
        }

        // No longer in que if transitioning too.
        if (newAttkQued && inBaseAnimTransition || !InAttkState)
            newAttkQued = false;
        // Reset cur attack if doing it & no new attack in que.
        if (DoingAttk && !newAttkQued && !inRollState) 
            curAttk = AttkType.none;

        anim.SetInteger("attkType", (int)curAttk);

        // Update last type of attack.
        if (curAttk != AttkType.none)
        {
            CurValidAttk = curAttk;
            anim.SetInteger("lastAttkType", (int)CurValidAttk);
        }
            
        
        // Reset attack once exit attack combo to movement.
        if (curAttk != AttkType.none && !DoingAttk && !inRollState)
        {
            curAttk = AttkType.none;
            quedAttk = AttkType.none;
            newAttkQued = false;
        }
    }        
    private AttkType quedAttk = AttkType.none;
    private AttkType curAttk = AttkType.none;
    /// <summary>
    /// If new attack has been qued as a follow-up.
    /// </summary>
    private bool newAttkQued = false;





    private void CombatRollAnim(bool doCombatRoll)
    {
        if (inRollState || !DoingCombatRoll)
            anim.SetBool("Roll", false);

        // If not already in a roll & is a roll request.
        if (!inRollState && doCombatRoll)                 
            anim.SetBool("Roll", true); 
    }
}