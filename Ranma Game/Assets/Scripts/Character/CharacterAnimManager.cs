using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimManager : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
        

    public bool CanDoDamage => anim.GetFloat("Damage") > 0;

    public AttkType GetCurAttack => (AttkType)anim.GetInteger("AttackType");

    

    public void DoAttack(AttkType attackType)
    {
        anim.SetInteger("AttackType", (int)attackType);
    }

    private void LateUpdate()
    {
        if(!CanDoDamage) anim.SetInteger("AttackType", (int)AttkType.none);
    }

}
