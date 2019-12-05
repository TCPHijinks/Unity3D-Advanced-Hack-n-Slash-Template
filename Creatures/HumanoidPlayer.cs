using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidPlayer : MonoBehaviour
{
    [SerializeField] private Humanoid humanoid;
    [SerializeField] private Camera cam;
    [SerializeField] private int rotDampening = 99;





    private void Start()
    {
        humanoid = GetComponent<Humanoid>();
    }
   




    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R) && humanoid.InCombat)
            humanoid.InCombat = false;
        else if (!humanoid.InCombat && (Input.GetKeyDown(KeyCode.R) || humanoid.Blocking || Input.GetMouseButtonDown(0)))
            humanoid.InCombat = true;


        Vector3 MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        bool running = false;
        // Update player move states (speed) based on button input.
        if (MoveDir != Vector3.zero)
        {
            if (Input.GetButton("Run") && humanoid.Grounded)
            {
                running = true;
                humanoid.SetMoveState(Humanoid.moveEnum.Run, MoveDir);
            }
            else if(humanoid.Grounded)
            {
                humanoid.SetMoveState(Humanoid.moveEnum.Walk, MoveDir);
            }
            else if(!humanoid.Grounded)
            {
                humanoid.SetMoveState(Humanoid.moveEnum.Jump, MoveDir);
            }
           

            if (humanoid.Blocking && !running)
                humanoid.Rotate(cam.transform.forward, rotDampening);
            else
                humanoid.RotateRelative(new Vector3(MoveDir.x, 0, MoveDir.z), cam.transform, rotDampening);
        }
        else
        {
            humanoid.SetMoveState(Humanoid.moveEnum.Idle, MoveDir);
        }
          

        // Try to jump on key press. *********
        if (Input.GetKey(KeyCode.Space))
            humanoid.Jump();


        // Humanoid will long jump if holding jump key. ******
        humanoid.amLongJmping = Input.GetKey(KeyCode.Space);

        // Attack when attack button clicked. 
        if (Input.GetMouseButtonDown(0))
        {          
            if (humanoid.BaseMoveStateMaxSpd > 0)
                humanoid.Attack(false);
            else humanoid.Attack(true);
        }            
        

        // Block with shield when block button clicked.
        if (Input.GetMouseButton(1))
            humanoid.Blocking = true;
        else
            humanoid.Blocking = false;
    }
}