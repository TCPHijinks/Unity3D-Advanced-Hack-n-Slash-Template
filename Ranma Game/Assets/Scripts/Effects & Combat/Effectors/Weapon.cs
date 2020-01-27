using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Effector, IDamages
{
    public bool CanDamage = false;  
    protected int baseDamage = 5;
 





    protected void Start()
    {
      
    }

    



    public int WeaponTypeCrushBonusDamage { get => _crushDmg; }
    protected int _crushDmg = 0;
    public int WeaponTypePierceBonusDamage { get => _pierceDmg; }
    protected int _pierceDmg = 0;
    public int WeaponTypeSlashBonusDamage { get => _slashDmg; }
    protected int _slashDmg = 0;
   



    /// <summary>
    /// Calculates current attack animation's dynamic damage.
    /// </summary>   
    /// <param name="dmgType"></param>
    /// <returns></returns>
    private int GetDamage(AttkType dmgType)
    {
        int typeDamage = 0;
        switch (dmgType) 
        {           
            case AttkType.crush:
                typeDamage += WeaponTypeCrushBonusDamage;
                break;
            case AttkType.pierce:
                typeDamage += WeaponTypePierceBonusDamage;
                break;
            case AttkType.slash:
                typeDamage += WeaponTypeSlashBonusDamage;
                break;
            default:
                return 0;
        }
        return baseDamage + typeDamage;
    }





    /// <summary>
    /// Adds all weapon effects to target and enables them.
    /// </summary>
    /// <param name="targetProperties"></param>
    private void DoEffects(CharModifyableProperties targetProperties)
    {
        // Save effect components to list as add them to target.
        var effects = new List<Effect>();
        foreach (System.Type effect in effectsAndAmounts.Keys)
            effects.Add(targetProperties.gameObject.AddComponent(effect) as Effect);

        // Set on with effect amount all just added effects on target.
        foreach (var e in effects)                             
            e.SetEffectOn(effectsAndAmounts[e.GetType()]);        
    }





    /// <summary>
    /// Adds new effect type to effects applied to target on hit. Stacks identical effects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="effect"></param>
    /// <param name="amount"></param>
    protected void AddNewEffect<T>(T effect, float amount) where T: System.Type
    {
        if (typeof(Effect) != effect.BaseType)
        {
            Debug.LogError("Weapon ERROR - Failed to add new Effect type. The " + effect + " is not an Effect type!");
            return;
        }

        // Add new effect or add effect amount to existing.
        if(effectsAndAmounts.ContainsKey(effect))        
            effectsAndAmounts[effect] = effectsAndAmounts[effect] + amount;        
        else        
            effectsAndAmounts.Add(effect, amount);        
    }
    private Dictionary<System.Type, float> effectsAndAmounts = new Dictionary<System.Type, float>();
    

   


    /// <summary>
    /// Applies dynamic attack damage + all other effects to target.
    /// </summary>
    /// <param name="targetProperties"></param>
    /// <param name="dmgType"></param>
    /// <param name="statDamageBonus"></param>
    public void DoDamage(CharModifyableProperties targetProperties, AttkType dmgType)
    {
        // Calculate dynamic damage.
        int damage = GetDamage(dmgType);
      
        // Apply dynamic damage to effects, then remove once applied to target.
        AddNewEffect(typeof(DamageEffect), damage);     
        DoEffects(targetProperties);
        
        AddNewEffect(typeof(DamageEffect), -damage);      
    }




     
    private void OnTriggerEnter(Collider other)
    {
        if (!CanDamage) return;
        DoDmgIfHitNewCreature(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!CanDamage) return;
        DoDmgIfHitNewCreature(other);
    }






    /// <summary>
    /// If new target is hit during attack, does damage/effects on target. Then this weapon can't effect target until attack animation finishes.
    /// </summary>
    /// <param name="other"></param>
    private void DoDmgIfHitNewCreature(Collider other)
    {        
        if (other.tag != targetTag) return;
        Debug.Log("Here");
        if (alreadyDamaged.Contains(other.gameObject)) return;
        alreadyDamaged.Add(other.gameObject);
        DoDamage(other.gameObject.GetComponent<CharModifyableProperties>(), AttkType.slash);  // ************* REPLACE 5 WITH STAT BONUS *****************
    }
    [SerializeField] string targetTag = "Enemy";
    List<GameObject> alreadyDamaged = new List<GameObject>();





    protected void Update()
    {      
        // Reset already hit list so can damage target again in next attack.
        if (alreadyDamaged.Count > 0)
            alreadyDamaged = new List<GameObject>();
    }
}
