using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimManager : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public bool CanDoDamage => anim.GetFloat("Damage") > 0;

    public AttkType GetCurAttack => (AttkType)anim.GetInteger("AttackType");

    public bool DoingAttack { get; private set; }

    public bool InAttkComboAndCanMove { get; private set; } = false;

    public float AnimMoveSpeedPenalty { get; private set; }

    public void DoAttack(AttkType attackType)
    {
        anim.SetInteger("AttackType", (int)attackType);
        if (!DoingAttack && !inBaseAnimTransition && attackType == AttkType.standard) _doChargedAttk = !_doChargedAttk;
    }

    private bool canDoCharged = false;

    public void CancelChargedAttack()
    {
        _doChargedAttk = false;
    }

    private bool _doChargedAttk = false;

    private void Update()
    {
        inBaseAnimTransition = anim.IsInTransition(0);
        if (inBaseAnimTransition) startedBaseTransition = true;
    }

    private bool startedBaseTransition = false;

    private void LateUpdate()
    {
        //  if (!CanDoDamage && !_doChargedAttk) anim.SetInteger("AttackType", (int)AttkType.none);
        InAttkComboAndCanMove = anim.GetCurrentAnimatorStateInfo(0).IsTag("CanMoveAttack");
        DoingAttack = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || InAttkComboAndCanMove;
        AnimMoveSpeedPenalty = anim.GetFloat("MoveSpeedPenaltyPercentage");
        anim.SetBool("ChargedAttack", _doChargedAttk);

        // If transition just finished to a new attack, reset attack in prep for next one.
        if (startedBaseTransition && !inBaseAnimTransition && anim.GetInteger("AttackType") != (int)AttkType.none)
        {
            startedBaseTransition = false;
            anim.SetInteger("AttackType", (int)AttkType.none);
        }
    }

    private bool inBaseAnimTransition = false;
}