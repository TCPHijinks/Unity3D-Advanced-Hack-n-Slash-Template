using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCurHpSpMpManager : MonoBehaviour
{
    Creature creature;
    private int stamina, health, magica;

    // Start is called before the first frame update
    void Start()
    {
        creature = GetComponent<Creature>();
        stamina = creature.MaxStamina;
        health = creature.MaxHealth;
        magica = creature.MaxMagica;
        Debug.Log(health);
    }

    
    public void UseStamina(int amount)
    {

    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Destroy(this);
    }

    public void HealDamage(int amount)
    {

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
