using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : Weapon
{    
    // Start is called before the first frame update
    void Awake()
    {
        baseDamage = 3;
        _heavyDmg = 100;
    }

    private new void Start()
    {
        base.Start();
        AddNewEffect(typeof(DamageEffect), 3);        
    }

    private new void Update()
    {
        base.Update();
    }



}
