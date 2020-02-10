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

    /// <summary>
    /// Returns damage animation curve from animator.
    /// </summary>
    public bool CanDoDamage => anim.GetFloat("Damage") > 0;

    /// <summary>
    /// Returns current attack being played by Animator (if any).
    /// </summary>
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
        Debug.Log(attackType);
        if (_inBaseAnimTransition) _updatedAttackDuringTransition = true;
        anim.SetInteger("AttackType", (int)attackType);
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
        Debug.Log(_justFinishedTransitionedToAttk);
        // Reset attack in prep for next one.
        if (_justFinishedTransitionedToAttk && !_updatedAttackDuringTransition)  // Reset attack if just transitioned w/o trying to attack again
        {
            anim.SetInteger("AttackType", (int)AttkType.none);
        }
        // Reset that gave attack request during transition once acted upon by not resetting attack above.
        if (_updatedAttackDuringTransition && !_inBaseAnimTransition) _updatedAttackDuringTransition = false;

        // If just finished transition to an attack, not start of transition.
        if (_justFinishedTransitionedToAttk) _startedBaseTransition = false;
    }

    /// <summary>
    /// Get and update animator parameters at end of each frame.
    /// </summary>
    private void LateUpdate()
    {
        InAttkComboAndCanMove = anim.GetCurrentAnimatorStateInfo(0).IsTag("CanMoveAttack");
        DoingAttack = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || InAttkComboAndCanMove;
        AnimMoveSpeedPenalty = anim.GetFloat("MoveSpeedPenaltyPercentage");
    }
}