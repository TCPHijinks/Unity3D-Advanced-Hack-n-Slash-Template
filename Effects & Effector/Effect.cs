using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    protected CreatureModifyableProperties creatureModifyable;
    public float amount;
    void Awake()
    {
        creatureModifyable = GetComponentInParent<CreatureModifyableProperties>();        
    }

    /// <summary>
    /// Destroy's the effect component.
    /// </summary>
    public void Destroy()
    {
        SetEffectOff();
        Destroy(this);
    }

    /// <summary>
    /// Disables effect, but doesn't destroy component.
    /// </summary>
    public virtual void SetEffectOff()
    {
        if (!_activeEffect) return;
        _activeEffect = false;
        UndoModify();
    }

    /// <summary>
    /// Enables the component's effect.
    /// </summary>
    /// <param name="target">Target of this single effect.</param>
    public virtual void SetEffectOn()
    {
        if (_activeEffect) return;
        _activeEffect = true;
        DoModify();
    }

    
    /// <summary>
    /// If the effect is currently active.
    /// </summary>
    /// <returns>If the effect is on.</returns> 
    public virtual bool EffectIsActive { get => _activeEffect; }
    private bool _activeEffect = false;

    protected abstract void DoModify();
    protected abstract void UndoModify();
}
