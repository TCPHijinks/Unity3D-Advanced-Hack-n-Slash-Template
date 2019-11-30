using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidPlayer : Humanoid
{
    [SerializeField] private Camera cam;
    [SerializeField] private int rotDampening = 99;
    
    [SerializeField] float defaultMoveSpd = .7f, runMoveSpd = 1.5f;


   

    new void Update()
    {
        
      
        // Rotates player to face Input direction.
        Rotate(InputMoveDir(), rotDampening, RotationSpeed);

        // Update player move states (speed) based on button input.
        if (MoveInputPressed)
        {            
            if (Input.GetButton("Run"))
            {
                baseMoveStateMaxSpd = runMoveSpd;
            }
            else
                baseMoveStateMaxSpd = defaultMoveSpd;
        }
        else
        {
            baseMoveStateMaxSpd = 0; // Idle.
        }
            

        // Try to jump on key press. *********
        if (Input.GetKey(KeyCode.Space))
            Jump();

        // Humanoid will long jump if holding jump key. **********
        amLongJmping = Input.GetKey(KeyCode.Space);

        // Attack when attack button clicked. ***********
        if (Input.GetMouseButtonDown(0))
        {          
            if (baseMoveStateMaxSpd > 0)
                Attack(false, 3);
            else Attack(true, 2);
        }            

        // Block with shield when block button clicked. *********
        if (Input.GetMouseButton(1))
            Blocking = true;
        else
            Blocking = false;

        // Update base humanoid.
        base.Update();
    }


    /// <summary>
    /// Return movement direction using player input and camera rotation.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 InputMoveDir()
    {
        // Don't update if in air or not pressing key.
        if (!MoveInputPressed)
            return transform.forward;

        Vector3 forward, right;
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Movement dirs are relative to the player camera.
        forward = cam.transform.forward.normalized;
        right = cam.transform.right.normalized;
        
        // Return new movement direction relative to cam.
        return (forward * inputDir.z + right * inputDir.x);
    }



    /// <summary>
    /// Returns true if the player is pressing either the Horizontal or Vertical keyboard input buttons.
    /// </summary>
    public bool MoveInputPressed => Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
}