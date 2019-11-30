using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerIdleState : StateMachineBehaviour
{
   // This does nothing itself. It's used by custom Animator updater classes to know if the gameobject is in idle via
   //  attempting to call GetCOmponent for 'LayerIdleState' (i.e. HumanoidAnim).
}
