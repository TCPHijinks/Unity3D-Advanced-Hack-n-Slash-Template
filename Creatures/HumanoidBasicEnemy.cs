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

    // Update is called once per frame
    void Update()
    {
        Vector3 MoveDir = target.transform.position - transform.position;
        humanoid.Rotate(MoveDir, 1);
        humanoid.SetMoveState(Humanoid.moveEnum.Walk);
        anim.UpateAnimator(humanoid.Grounded, false, humanoid.InCombat, humanoid.CurSpeed, false, MoveDir.x, MoveDir.z, HumanoidAnim.AttkType.none, false, 0, false);
    }
}
