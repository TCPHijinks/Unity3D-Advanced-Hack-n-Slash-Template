using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 0F;//10.0F;
    private const float Y_ANGLE_MAX = 50.0F;
 //   private const float CAM_DISTANCE_MIN = 2f;
 //   private const float CAM_DISTANCE_MAX = 5f;

    public Transform player;
    public Transform _camera;

    [SerializeField] private float distance = 20f;
    private float currentX = 0f;
    [SerializeField] private float currentY = 34f;  
    public float smoothAmount = 5f;



    void Start()
    {
        //Set up things on the start method
        _camera = transform;    

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    private void Update()
    {
      
    }


   
    void LateUpdate()
    {
        //makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);


        // Put camera on player, apply the rotation * direction.
        _camera.position = Vector3.Lerp(transform.position, player.position + rotation * dir, Time.deltaTime * smoothAmount);

      
        _camera.LookAt(player);
    }
}

