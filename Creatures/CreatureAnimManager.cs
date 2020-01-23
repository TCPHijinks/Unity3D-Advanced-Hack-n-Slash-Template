using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimManager : MonoBehaviour
{
    protected CharacterController cc;
    protected Animator anim;
    protected float defaultColliderHeight;

    public int StunLevel { get; set; } = 0;
    public bool IsStunned { get; protected set; } = false;
    public bool DoingAttk { get; protected set; }    
    public bool InAttkState { get; protected set; } = false;
    public bool DoingOffhandAttack { protected set; get; }
    /// <summary>
    /// Penalty to prevent/limit rotation when committed to an attack.
    /// </summary>
    public float AttkRotationPenalty { get; protected set; }
    /// <summary>
    /// Percentage of attack damage done based on attack animation curves.
    /// </summary>
    public float AttkDmgPercentage => (int)anim.GetFloat("AttkDmg");
    /// <summary>
    /// Bonus damage that can be given by attack animations.
    /// </summary>
    public float ComboBonusDmg => (int)anim.GetFloat("ComboBonusDmg");
    /// <summary>
    /// Last AttkType attack passed to animator that wasn't none.
    /// </summary>
    public AttkType CurValidAttk { get; protected set; }
    


    public void DoStun(Transform stunSrc, Transform stunTarget, int stunStrength1to3)
    {
        Stun stun = new Stun(stunSrc, stunTarget, Mathf.Clamp(stunStrength1to3, 1, 3));
        StunLevel = stun.StunLev;
        Debug.Log(stun.StunMoveDir);
        anim.SetInteger("StunDir", (int)stun.StunMoveDir);
    }


    protected struct Stun
    {
        public int StunLev { get; private set; }
        public Direction StunMoveDir { get; private set; }
                

        public Stun(Transform stunSrc, Transform stunTarget, int stunLevel_1to3) : this()
        {
            StunLev = stunLevel_1to3;
            StunMoveDir = CalcStunMoveDir(stunSrc, stunTarget);
        }

        private Direction CalcStunMoveDir(Transform src, Transform target)
        {
            Direction direction = Direction.None;
            Vector3 srcToTarget = (target.position - src.position).normalized;

            
            Direction vertDir = GetVertDir(srcToTarget, target, .1f); 
            Direction horzDir = GetHorzDir(srcToTarget, target, .2f);
                      
            direction = (horzDir > vertDir) ? horzDir : vertDir;

            if(direction == Direction.None)
                direction += (int)GetVertDir(srcToTarget, target, 0f);

            Debug.Log(direction);
            return direction;
        }

        private Direction GetVertDir(Vector3 srcToTarget, Transform target, float threshold)
        {
            if (Vector3.Dot(srcToTarget, target.forward) < -threshold)
                return Direction.Fwd;
            else if (Vector3.Dot(srcToTarget, target.forward) > threshold)
                return Direction.Back;
            return Direction.None;
        }

        private Direction GetHorzDir(Vector3 srcToTarget, Transform target, float threshold)
        {          
            if (Vector3.Dot(srcToTarget, target.right) < -threshold)
                return Direction.Right;
            else if (Vector3.Dot(srcToTarget, target.right) > threshold)
                return Direction.Left;
            return Direction.None;
        }
    }





    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        defaultColliderHeight = cc.height;
    }
}
