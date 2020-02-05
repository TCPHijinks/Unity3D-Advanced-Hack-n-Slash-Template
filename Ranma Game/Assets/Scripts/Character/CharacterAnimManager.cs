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

    public AttkType CurAttack => (AttkType)anim.GetInteger("AttackType");

    /// <summary>
    /// Return whether in any Animator state with an "Attack" or "CanMoveAttack" tag.
    /// </summary>
    public bool DoingAttack { get; private set; }

    /// <summary>
    /// Return whether in an Animator state tagged "CanMoveAttack".
    /// </summary>
    public bool InAttkComboAndCanMove { get; private set; } = false;

    /// <summary>
    /// Return penalty percentile(%) to speed (0.0 to 1.0) from Animator animation curves.
    /// </summary>
    public float AnimMoveSpeedPenalty { get; private set; }

    private bool _inBaseAnimTransition = false;
    private bool _updatedAttackDuringTransition = false;
    private bool _justFinishedTransitionedToAttk = false;
    private bool _startedBaseTransition = false;

    /// <summary>
    /// Request Animator to play specified Attack Type.
    /// </summary>
    /// <param name="attackType"></param>
    public void DoAttack(AttkType attackType)
    {
        _canDoAttack = !_canDoAttack; // TEMP - Toggles for player so Button Up & Button Down don't call Attack() twice.
        if (_inBaseAnimTransition && _canDoAttack) _updatedAttackDuringTransition = true;
        if (_canDoAttack) anim.SetInteger("AttackType", (int)attackType);
    }

    // If to do charged attack, and whether able to.
    private bool doChargedAttk = false, _canDoAttack = false;

    /// <summary>
    /// Cancel charged attack.
    /// </summary>
    public void SetCancelChargedAttack()
    {
        doChargedAttk = false;
    }

    /// <summary>
    /// Update animator parameters at start of each frame.
    /// </summary>
    private void Update()
    {
        // Check if base layer (0) is in a transition between animation states.
        _inBaseAnimTransition = anim.IsInTransition(0);

        // Started transition.
        if (!_startedBaseTransition && _inBaseAnimTransition) _startedBaseTransition = true;

        // Just transitioned if transition started, ended, and now in an Attack.
        _justFinishedTransitionedToAttk = DoingAttack && _startedBaseTransition && !_inBaseAnimTransition;

        // Reset attack in prep for next one.
        if (!_canDoAttack && _justFinishedTransitionedToAttk && !_updatedAttackDuringTransition ||  // Reset attack if just transitioned w/o trying to attack again
            !_canDoAttack && _startedBaseTransition && !_inBaseAnimTransition && !DoingAttack)// OR if can't attack and just exited attack state w/o trying to attack again.
        {
            anim.SetInteger("AttackType", (int)AttkType.none);
        }
        // Reset that gave attack request during transition once acted upon by not resetting attack above.
        if (_updatedAttackDuringTransition && !_inBaseAnimTransition) _updatedAttackDuringTransition = false;

        // If just finished transition to an attack, not start of transition.
        if (_justFinishedTransitionedToAttk) _startedBaseTransition = false;

        // If can do an attack (button down) and just transitioned into an attack.
        if (_canDoAttack && _justFinishedTransitionedToAttk)
        {
            doChargedAttk = true;
        }
        // Can't charge attack if not able to attack and no attack executing.
        else if (!_canDoAttack && CurAttack != AttkType.none) doChargedAttk = false;
    }

    /// <summary>
    /// Get and update animator parameters at end of each frame.
    /// </summary>
    private void LateUpdate()
    {
        InAttkComboAndCanMove = anim.GetCurrentAnimatorStateInfo(0).IsTag("CanMoveAttack");
        DoingAttack = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || InAttkComboAndCanMove;
        AnimMoveSpeedPenalty = anim.GetFloat("MoveSpeedPenaltyPercentage");
        anim.SetBool("ChargedAttack", doChargedAttk);
    }
}