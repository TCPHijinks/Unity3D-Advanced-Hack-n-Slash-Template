using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidBasicEnemy : MonoBehaviour
{
    public GameObject target;
    private HumanoidAnim anim;
    [SerializeField] private Humanoid humanoid;

    // Start is called before the first frame update
    void Start()
    {
        humanoid = GetComponent<Humanoid>();
        anim = GetComponent<HumanoidAnim>();
        humanoid.InCombat = false;
    }


    void Update()
    {
        Vector3 MoveDir = target.transform.position - transform.position;
        humanoid.Rotate(MoveDir, 1);
        humanoid.SetMoveState(Humanoid.moveEnum.Walk);
        
        AttkType attk = AttkType.none;
        if (Vector3.Distance(transform.position, target.transform.position) < 2)
        {
            humanoid.InCombat = true;
            attk = AttkType.crush;
        }
        else
        {
            humanoid.InCombat = false;
        }
            

        anim.UpateAnimator(humanoid.Grounded, false, humanoid.InCombat, humanoid.CurSpeed, false, MoveDir.x, MoveDir.z, attk, false, 0, false);
    }
}

