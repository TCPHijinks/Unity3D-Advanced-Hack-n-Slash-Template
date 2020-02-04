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
        if (attackType == AttkType.standard) _doChargedAttk = !_doChargedAttk;
    }

    public void CancelChargedAttack()
    {
        _doChargedAttk = false;
    }

    private bool _doChargedAttk = false;

    private void Update()
    {
    }

    private void LateUpdate()
    {
        if (!CanDoDamage && !_doChargedAttk) anim.SetInteger("AttackType", (int)AttkType.none);
        InAttkComboAndCanMove = anim.GetCurrentAnimatorStateInfo(0).IsTag("CanMoveAttack");
        DoingAttack = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || InAttkComboAndCanMove;
        AnimMoveSpeedPenalty = anim.GetFloat("MoveSpeedPenaltyPercentage");
        anim.SetBool("ChargedAttack", _doChargedAttk);
    }
}