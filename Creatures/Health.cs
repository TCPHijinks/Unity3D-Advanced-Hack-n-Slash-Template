using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Creature self;
    private HumanoidAnim anim;
    public int CurHP { get; private set; }
   
    void Start()
    {
        self = GetComponent<Creature>();
        anim = GetComponent<HumanoidAnim>();
        CurHP = self.MaxHealth;
    }


    public void TakeDamage(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("ERROR - Health damage amount was < 0. All damage must be positive to be applied.");
            return;
        }
        if(amount > 0)
        {            
            Debug.Log(gameObject.name + " was damaged! (" + amount + "). Remainder " + (CurHP - amount) + ".");
        }
            
        CurHP -= amount;
        if (CurHP <= 0) Destroy(gameObject);        
    }

 
}
