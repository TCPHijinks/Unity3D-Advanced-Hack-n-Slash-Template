using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Effector, IDamages
{
    HumanoidAnim humanoidAnim; // TO DO: Replace so moore generic to use with all creatures.

    private void Start()
    {
        humanoidAnim = GetComponentInParent<HumanoidAnim>();
    }

    

    public int CrushingDamage { get => _crushDmg; }
    protected int _crushDmg = 0;
    public int PiercingDamage { get => _pierceDmg; }
    protected int _pierceDmg = 0;
    public int SlashingDamage { get => _slashDmg; }
    protected int _slashDmg = 0;
   

    private int GetDamage(int amountBonus, AttkType dmgType)
    {
        switch (dmgType)
        {
            case AttkType.crush:
                return amountBonus + CrushingDamage;
            case AttkType.pierce:
                return amountBonus + PiercingDamage;
            case AttkType.slash:
                return amountBonus + SlashingDamage;
            default:
                return 0;
        }        
    }

    public void DoDamage(CreatureModifyableProperties targetProperties, AttkType dmgType, int amount)
    {
       
            
            AddEffectToTarget(targetProperties.Creature, typeof(DamageEffect));
            Effect effect = targetProperties.gameObject.GetComponent<DamageEffect>();
            effect.amount = GetDamage(amount, dmgType);
            effect.SetEffectOn();
        
    }


    private void Update()
    {
        if (!humanoidAnim.DoingAttk && alreadyDamaged.Count > 0)
            alreadyDamaged.Clear();
    }


    [SerializeField] string targetTag = "Enemy";
    List<GameObject> alreadyDamaged = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {        
        CheckIfAttkHitObject(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckIfAttkHitObject(other);
    }

    void CheckIfAttkHitObject(Collider other)
    {        //
        if (humanoidAnim.CurValidAttk != AttkType.none && humanoidAnim.AttkRotationPenalty > 0 && other.tag == targetTag && humanoidAnim.DoingAttk)
        {
            if (!alreadyDamaged.Contains(other.gameObject))
            {
                alreadyDamaged.Add(other.gameObject);
                DoDamage(other.gameObject.GetComponent<CreatureModifyableProperties>(), humanoidAnim.CurValidAttk, 5);                
            }
        }
    }
}
