using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHpSpMpManager : MonoBehaviour
{
    Creature creature;
    private int stamina, health, magica;
    // Start is called before the first frame update
    void Start()
    {
        creature.GetComponent<Creature>();
        stamina = creature.MaxStamina;
        health = creature.MaxHealth;
        magica = creature.MaxMagica;
    }

    
    public void UseStamina(int amount)
    {

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
