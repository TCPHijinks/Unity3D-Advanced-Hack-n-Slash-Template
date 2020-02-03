using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Effector, IDamages
{
    private CharacterAnimManager animManager;
    public bool CanDamage = false;
    protected int baseDamage = 5;

    protected void Start()
    {
        animManager = GetComponentInParent<CharacterAnimManager>();
    }

    public int WeaponTypeStandardBonusDamage { get => _stdDmg; }
    protected int _stdDmg = 0;
    public int WeaponTypeHeavyBonusDamage { get => _heavyDmg; }
    protected int _heavyDmg = 0;
    public int WeaponTypeChargedBonusDamage { get => _chargedDmg; }
    protected int _chargedDmg = 0;

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
            case AttkType.standard:
                typeDamage += WeaponTypeStandardBonusDamage;
                break;

            case AttkType.heavy:
                typeDamage += WeaponTypeHeavyBonusDamage;
                break;

            case AttkType.charged:
                typeDamage += WeaponTypeChargedBonusDamage;
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
    protected void AddNewEffect<T>(T effect, float amount) where T : System.Type
    {
        if (typeof(Effect) != effect.BaseType)
        {
            Debug.LogError("Weapon ERROR - Failed to add new Effect type. The " + effect + " is not an Effect type!");
            return;
        }

        // Add new effect or add effect amount to existing.
        if (effectsAndAmounts.ContainsKey(effect))
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
    public void DoDamage(CharModifyableProperties targetProperties, AttkType dmgType)
    {
        // Calculate dynamic damage.
        int damage = GetDamage(dmgType);

        // Apply dynamic damage to effects, then remove once applied to target.
        AddNewEffect(typeof(DamageEffect), damage);
        DoEffects(targetProperties);

        // Knockback.
        targetProperties.character.UpdateKnockbackRequest(knockback + (damage * 3), GetComponentInParent<Transform>().position);

        AddNewEffect(typeof(DamageEffect), -damage);
    }

    [SerializeField] private float knockback = 70;

    private void OnTriggerEnter(Collider other)
    {
        if (!CanDamage && animManager.CanDoDamage) return;
        DoDmgIfHitNewCreature(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!CanDamage && animManager.CanDoDamage) return;
        DoDmgIfHitNewCreature(other);
    }

    /// <summary>
    /// If new target is hit during attack, does damage/effects on target. Then this weapon can't effect target until attack animation finishes.
    /// </summary>
    /// <param name="other"></param>
    private void DoDmgIfHitNewCreature(Collider other)
    {
        if (other.tag != targetTag) return;
        if (!animManager.CanDoDamage) return;
        if (alreadyDamaged.Contains(other.gameObject)) return;
        alreadyDamaged.Add(other.gameObject);
        DoDamage(other.gameObject.GetComponent<CharModifyableProperties>(), animManager.GetCurAttack);
    }

    [SerializeField] private string targetTag = "Enemy";
    private List<GameObject> alreadyDamaged = new List<GameObject>();

    protected void Update()
    {
        // Reset already hit list so can damage target again in next attack.
        if (alreadyDamaged.Count > 0 && !animManager.CanDoDamage)
            alreadyDamaged = new List<GameObject>();
    }
}