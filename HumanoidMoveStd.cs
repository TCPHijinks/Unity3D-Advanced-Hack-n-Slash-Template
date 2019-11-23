using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidMoveStd : MonoBehaviour
{
    protected CharacterController controller; // Character environment info getting.

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


  
    /// <summary>
    /// Returns new dynamic max speed and updates prevMaxSpd reference with any updates to the maximum speed.
    /// </summary>
    /// <param name="mxSpeed"></param>
    /// <param name="jogSpd"></param>
    /// <param name="minMoveSpd"></param>
    /// <param name="curVel"></param>
    /// <param name="gndSpdMod"></param>
    /// <param name="prevMaxSpd"></param>
    /// <returns></returns>
    public float Move(float mxSpeed, float jogSpd, float minMoveSpd, float curVel, float gndSpdMod, ref float prevMaxSpd)
    {
        float newMax = 0;
        // Max speed more/less depending on terrain incline.
        if (mxSpeed > 0 && controller.velocity.y >= 0)
            newMax = minMoveSpd + (mxSpeed + curVel) - gndSpdMod; // Less top speed if go up incline.
        else if (mxSpeed > 0)
            newMax = minMoveSpd + (mxSpeed + curVel) + gndSpdMod; // More top speed if down incline.

        // Clamp min/top max speed.
        newMax = Mathf.Clamp(newMax, 0, mxSpeed * 1.6f);

        // Update max speed if significant difference.
        if (Mathf.Abs(prevMaxSpd - newMax) >= minMoveSpd)
        {
            if (mxSpeed > 0 && newMax < (jogSpd * .8f)) // If moving, enforce not too slow.   
                newMax = jogSpd * .8f;

            prevMaxSpd = newMax;
            return newMax;
        }
        return prevMaxSpd;
    }
}
