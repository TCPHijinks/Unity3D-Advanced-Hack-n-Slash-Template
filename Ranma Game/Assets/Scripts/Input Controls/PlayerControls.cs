// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input Controls/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""b27f94c3-7a48-481e-a4db-d1812bc40329"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""a67becc1-e7e1-4c40-a523-1a133ffa993a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackStd"",
                    ""type"": ""Button"",
                    ""id"": ""25bd8a32-f6ff-4b6b-b7cb-ad5dc64f0782"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackHeavy"",
                    ""type"": ""Button"",
                    ""id"": ""f2cecc6d-a69b-4bb9-a599-6f22089573f5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Maneuver"",
                    ""type"": ""Button"",
                    ""id"": ""ff53ebcb-5a75-4308-bef4-325c08e9e9e8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""959dd68e-be84-4378-92e6-c0f2daf0d3fb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2db0fc3b-815d-43f9-a16e-3e526d39a2f8"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16275249-edf0-4be1-af05-0865d3d6bd88"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackStd"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b0465ff-7c05-4eb0-b104-2cbf086d56eb"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackHeavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2cd00bfa-eb1d-443a-be81-8febe9bcc5b0"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Maneuver"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f0f316e-09ba-4a03-9d16-3c29910c7a09"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_AttackStd = m_Gameplay.FindAction("AttackStd", throwIfNotFound: true);
        m_Gameplay_AttackHeavy = m_Gameplay.FindAction("AttackHeavy", throwIfNotFound: true);
        m_Gameplay_Maneuver = m_Gameplay.FindAction("Maneuver", throwIfNotFound: true);
        m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_AttackStd;
    private readonly InputAction m_Gameplay_AttackHeavy;
    private readonly InputAction m_Gameplay_Maneuver;
    private readonly InputAction m_Gameplay_Interact;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @AttackStd => m_Wrapper.m_Gameplay_AttackStd;
        public InputAction @AttackHeavy => m_Wrapper.m_Gameplay_AttackHeavy;
        public InputAction @Maneuver => m_Wrapper.m_Gameplay_Maneuver;
        public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @AttackStd.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackStd;
                @AttackStd.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackStd;
                @AttackStd.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackStd;
                @AttackHeavy.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackHeavy;
                @AttackHeavy.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackHeavy;
                @AttackHeavy.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackHeavy;
                @Maneuver.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnManeuver;
                @Maneuver.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnManeuver;
                @Maneuver.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnManeuver;
                @Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @AttackStd.started += instance.OnAttackStd;
                @AttackStd.performed += instance.OnAttackStd;
                @AttackStd.canceled += instance.OnAttackStd;
                @AttackHeavy.started += instance.OnAttackHeavy;
                @AttackHeavy.performed += instance.OnAttackHeavy;
                @AttackHeavy.canceled += instance.OnAttackHeavy;
                @Maneuver.started += instance.OnManeuver;
                @Maneuver.performed += instance.OnManeuver;
                @Maneuver.canceled += instance.OnManeuver;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAttackStd(InputAction.CallbackContext context);
        void OnAttackHeavy(InputAction.CallbackContext context);
        void OnManeuver(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
}
