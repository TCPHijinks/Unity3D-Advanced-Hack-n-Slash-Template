using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Creature self;
    public int CurHP { get; private set; }
   
    void Start()
    {
        self = GetComponent<Creature>();
        CurHP = self.MaxHealth;
    }


    public void TakeDamage(int amount)
    {      
        if(amount > 0)
        Debug.Log(gameObject.name + " was damaged! (" + amount + "). Remainder " + (CurHP - amount) + ".");
        CurHP -= Mathf.Abs(amount);
        if (CurHP <= 0) Destroy(gameObject);
    }

 
}
