using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnim : MonoBehaviour
{
    private CharacterController cc;
    private Animator anim;

    private int combatRollState;
    private int jumpState;
    private int combatMoveState;

    private int baseLayer;
    private int blockLayer;
    private int curLayer;
    private int curState;

    public bool DoingCombatRoll { get; private set; }
    public bool DoingSlide { get; private set; }
    public bool DoingAttk { get; private set; }
    public bool CommittedToAttack { get; private set; } = false;
    private bool inBaseAnimTransition = false;
    private bool inRollState = false;
    private bool inSlideState = false;
    public bool InAttkState { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
       

        combatRollState = Animator.StringToHash("Base Layer.Combat_Roll");
        jumpState = Animator.StringToHash("Base Layer.Jump");
        combatMoveState = Animator.StringToHash("Base Layer.Combat_Movement");
        baseLayer = anim.GetLayerIndex("Base Layer");
        blockLayer = anim.GetLayerIndex("Blocking Layer");
    }


    public float AttkRotationPenalty { get; private set; }

    bool dd = false;
    int attkVariant = 1;
  
    AnimatorStateInfo prevState;
 
    public void UpateAnimator(
        bool grounded, bool block, bool inCombat,
        float curSpd, bool running, float horizontal, float vertical, 
        AttkType attk, bool doCmbtRoll, int rollDir, bool crouching)
    {
        inBaseAnimTransition = anim.IsInTransition(baseLayer);
        curState = anim.GetCurrentAnimatorStateInfo(baseLayer).shortNameHash;

        // Combat rolling animation status.
        inRollState = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Roll");
        DoingCombatRoll = inRollState && !inBaseAnimTransition;

        // Sliding animation status.
        inSlideState = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Slide");
        DoingSlide = inSlideState && !inBaseAnimTransition;

        InAttkState = anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag("Attack");
        DoingAttk = InAttkState && !inBaseAnimTransition;


       
       

        AttkRotationPenalty = anim.GetFloat("AttkRotationPenalty");


        CombatRollAnim(doCmbtRoll);
      //  SlideAnim(crouching, grounded, curSpd);

        // Jumping if can't jump and not on cooldown after landing.
        anim.SetFloat("Speed", curSpd);
        anim.SetBool("OnGround", grounded);
        anim.SetBool("InCombat", inCombat);
        anim.SetBool("Crouching", crouching);
        anim.SetBool("Running", running);
        anim.SetFloat("Horizontal", horizontal, .045f, Time.deltaTime);
        anim.SetFloat("Vertical", vertical, .045f, Time.deltaTime);
        anim.SetInteger("RollDir", rollDir);
        anim.SetBool("InAttkState", InAttkState);
        cc.height = anim.GetFloat("Collider_Scale");
        if (cc.height == 0) cc.height = 1f;
        cc.center = new Vector3(cc.center.x, cc.height * .6f, cc.center.z);
     
        
        AttackAnim(attk);
        
       // if (curAttk != AttkType.none && block || inAttkState)
       //     anim.SetLayerWeight(blockLayer, 0);
       // else if(block || !inAttkState)
        //    anim.SetLayerWeight(blockLayer, 1);
        anim.SetBool("Block", block);
    }


   
   
    void AttackAnim(AttkType requestedAttk)
    {
        // Update que if valid request & not already transitioning to next anim.
        if (requestedAttk != AttkType.none && !inBaseAnimTransition)        
            quedAttk = requestedAttk;
        

        // Update que & cur attk if not doing attk or que request changes.
        if (curAttk == AttkType.none || !DoingAttk || curAttk != AttkType.none && requestedAttk != AttkType.none && requestedAttk != curAttk)
        {
            if (quedAttk != AttkType.none)
                newAttkQued = true;
            curAttk = quedAttk;
            quedAttk = AttkType.none;        
        }

        if (newAttkQued && InAttkState && inBaseAnimTransition)
            newAttkQued = false;
        if (!newAttkQued && (DoingAttk || InAttkState && inBaseAnimTransition))
            curAttk = AttkType.none;
        
        anim.SetInteger("attkType", (int)curAttk);
    }
    public enum AttkType { none, swing, overhead, lunge };    
    private AttkType quedAttk = AttkType.none;
    private AttkType curAttk = AttkType.none;
    private bool newAttkQued = false;




    void CombatRollAnim(bool doCombatRoll)
    {
        // If not already in a roll & is a roll request.
        if (!inRollState && doCombatRoll)                 
            anim.SetBool("Roll", true);        

        // Toggle off roll in Animator once started animation.
        if (DoingCombatRoll || inRollState)
            anim.SetBool("Roll", false);        
    }
}