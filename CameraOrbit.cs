using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 10.0F;
    private const float Y_ANGLE_MAX = 50.0f;
    private const float CAM_DISTANCE_MIN = 2f;
    private const float CAM_DISTANCE_MAX = 5f;

    public Transform player;
    public Transform _camera;

    private float distance = 10.0f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float sensitivityX = 4f;
    private float sensitivityY = 1f;
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
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY -= Input.GetAxis("Mouse Y") * sensitivityY;        
        distance -= Input.GetAxis("Mouse ScrollWheel") * 5;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        distance = Mathf.Clamp(distance, CAM_DISTANCE_MIN, CAM_DISTANCE_MAX);
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

