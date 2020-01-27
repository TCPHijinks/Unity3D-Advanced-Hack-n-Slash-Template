using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{ 
    [SerializeField] int maxHealth = 10;
    public int CurHP { get; private set; }
   
    void Start()
    {    
        CurHP = maxHealth;
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
