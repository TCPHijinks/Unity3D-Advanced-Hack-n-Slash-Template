using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar : Character
{
    private PlayerControls controls;
    private Vector2 moveDir = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animManager = GetComponent<CharacterAnimManager>();
        groundedCheck = GetComponent<GroundedCheck>();

        controls = new PlayerControls();
    }

    private void Update()
    {
        // Don't move if attacking.
        MoveAndRotate(moveDir);
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();

        controls.Gameplay.Maneuver.performed += ctx => Maneuver();
        // controls.Gameplay.Maneuver.performed += ctx => ManeuverEnd();
        controls.Gameplay.AttackStd.performed += ctx => AttackStd();
        controls.Gameplay.AttackHeavy.performed += ctx => AttackHeavy();
        controls.Gameplay.Interact.performed += ctx => Interact();

        controls.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveDir = Vector2.zero;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}