using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effector : MonoBehaviour
{
    // Take a target.
    // Apply all IEffectable to target.
    // IEffectable manages its own deletion.
    public GameObject target;

    [SerializeField] List<Effect> Effects = new List<Effect>();

    /// <summary>
    /// Applies all effects to target creature's gameobject as components.
    /// </summary>
    /// <param name="target"></param>
    public void UseEffector(Creature target)
    {       
        foreach (Effect effect in Effects)
        {
            target.gameObject.AddComponent(effect.GetType());
        }
              
    }
}
