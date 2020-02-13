using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Transform camera;

    [SerializeField] private float distance = 20f;
    private float currentX = 0f;
    [SerializeField] private float currentY = 34f;
    public float smoothAmount = 5f;

    private void Start()
    {
        //Set up things on the start method
        camera = transform;
        //   Cursor.lockState = CursorLockMode.Locked;
        //   Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // Distance offset and rotation offset.
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Put camera on player, apply the rotation * direction.
        camera.position = Vector3.Lerp(transform.position, player.position + rotation * dir, Time.deltaTime * smoothAmount);

        camera.LookAt(player);
    }
}