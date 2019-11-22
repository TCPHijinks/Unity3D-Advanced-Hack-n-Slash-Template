using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHumanoid : Humanoid
{
    [SerializeField] protected Camera cam;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
      
        Rotate(InputMoveDir(), 99, 6);

        if (MoveInputPressed) // Move speed states.
        {
            if (Input.GetButton("Run"))
                Move(runSpd);
            else
                Move(jogSpd);
        }

        if (Input.GetKey(KeyCode.Space))
            Jump();

        
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




    public bool MoveInputPressed => Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
}
