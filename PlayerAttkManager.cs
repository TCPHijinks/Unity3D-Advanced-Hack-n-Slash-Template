using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttkManager : MonoBehaviour
{
    [SerializeField] private GameObject rightPrimaryWep, leftOffhandWep;

    Creature creature;
   

    // Start is called before the first frame update
    void Start()
    {
        creature = GetComponent<Creature>();
        
    }


    AttkType lastValidAttk;
    // Update is called once per frame
    void Update()
    {
        EquipedInCombat();      
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
    HumanoidAnim anim;
}
