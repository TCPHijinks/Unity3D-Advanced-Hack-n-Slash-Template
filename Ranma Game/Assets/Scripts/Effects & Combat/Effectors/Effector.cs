using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effector : MonoBehaviour
{
    /// <summary>
    /// Applies all effects to target creature's gameobject as components.
    /// </summary>
    /// <param name="target"></param>
    protected void AddEffectToTarget(Character target, System.Type effect)
    {
        if (typeof(Effect) != effect.BaseType) return;

    
        target.gameObject.AddComponent(effect.GetType());
    }
}
