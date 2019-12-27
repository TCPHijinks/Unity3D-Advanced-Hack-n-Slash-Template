using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidEventHandler : MonoBehaviour
{
    public bool justCommitted = false;

    public void AttackEvent()
    {
        Debug.Log("Now committed.");
        justCommitted = true;
    }
}
