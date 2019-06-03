using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 10.0F;
    private const float Y_ANGLE_MAX = 50.0f;
    private const float CAM_DISTANCE_MIN = 4.5F;
    private const float CAM_DISTANCE_MAX = 6.5f;

    public Transform player;
    public Transform camera;

    private Camera cam;

    private float distance = 10.0f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float sensitivityX = 4f;
    private float sensitivityY = 1f;
    public float smoothAmount = 5f;
    void Start()
    {//Set up things on the start method
        camera = transform;
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY += Input.GetAxis("Mouse Y") * sensitivityY;        
        distance -= Input.GetAxis("Mouse ScrollWheel") * 5;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        distance = Mathf.Clamp(distance, CAM_DISTANCE_MIN, CAM_DISTANCE_MAX);

    }

    void LateUpdate()
    {//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        // Put camera on player, apply the rotation * direction.
        camera.position = Vector3.Lerp(transform.position, player.position + rotation * dir, Time.deltaTime * smoothAmount);


         //   = player.position + rotation * dir;
        camera.LookAt(player);
    }
}

