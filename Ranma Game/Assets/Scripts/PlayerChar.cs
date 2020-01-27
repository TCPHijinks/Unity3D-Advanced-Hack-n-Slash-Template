using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar : Character
{
    private PlayerControls controls;
    private Vector2 moveDir = Vector2.zero;
    private void Awake()
    {
        cControl = GetComponent<CharacterController>();

        controls = new PlayerControls();
        controls.Gameplay.Maneuver.performed += ctx => Maneuver();
        controls.Gameplay.AttackStd.performed += ctx => AttackStd();
        controls.Gameplay.AttackHeavy.performed += ctx => AttackHeavy();
        controls.Gameplay.Interact.performed += ctx => Interact();

        controls.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>();//  moveDir = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        controls.Gameplay.Move.canceled += ctx => moveDir = Vector2.zero;
    }

    private void Update()
    {
        // Don't move if attacking.
        MoveAndRotate(moveDir);
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Maneuver()
    {
        Debug.Log("JUMP! ROLL!");
    }

    void AttackStd()
    {
        Debug.Log("BASIC BITCH PUNCH!");
    }

    void AttackHeavy()
    {
        Debug.Log("KICK TO THE DICK");
    }

    void Interact()
    {
        Debug.Log("JUST USE IT!");
    }
}
