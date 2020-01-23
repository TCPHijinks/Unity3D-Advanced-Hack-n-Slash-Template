using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttkManager : MonoBehaviour
{
    [SerializeField] private Weapon rightPrimaryWep, leftOffhandWep;
    CreatureAnimManager anim;
    Creature creature;
   

    // Start is called before the first frame update
    void Start()
    {
        creature = GetComponent<Creature>();
        anim = GetComponent<HumanoidAnim>();
    }


   
    // Update is called once per frame
    void Update()
    {
        EquipedInCombat();

        if (!anim.DoingAttk)
        {
            leftOffhandWep.CanDamage = false;
            rightPrimaryWep.CanDamage = false;
            return;
        }


        if(anim.DoingOffhandAttack)
        {
            leftOffhandWep.CanDamage = true;
            rightPrimaryWep.CanDamage = false;
        }
        else
        {
            leftOffhandWep.CanDamage = false;
            rightPrimaryWep.CanDamage = true;
        }
    }

    void EquipedInCombat()
    {
        if (!creature.InCombat)
        {
            leftOffhandWep.gameObject.SetActive(false);
            rightPrimaryWep.gameObject.SetActive(false);
        }
        else
        {
            leftOffhandWep.gameObject.SetActive(true);
            rightPrimaryWep.gameObject.SetActive(true);
        }
    }
   
}
