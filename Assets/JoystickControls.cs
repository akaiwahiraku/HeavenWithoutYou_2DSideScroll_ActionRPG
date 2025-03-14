//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/JoystickControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @JoystickControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @JoystickControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""JoystickControls"",
    ""maps"": [
        {
            ""name"": ""Joystick"",
            ""id"": ""8f0abd78-5fdc-4dfe-97bf-bf6d24e28806"",
            ""actions"": [
                {
                    ""name"": ""JoystickMove"",
                    ""type"": ""Button"",
                    ""id"": ""bbf55dcb-2330-4559-b24d-2415fae9e80a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ced08b02-2793-4680-aba7-7e6e29d14ad0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""e92f9f54-0d41-4dcc-9d4c-37acbf072275"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""abc0a2a4-f028-4932-9f6c-bf07cc2e12c3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill"",
                    ""type"": ""Button"",
                    ""id"": ""9c70b0fe-6410-4fd6-9cb4-99ef030b5ba3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Guard"",
                    ""type"": ""Button"",
                    ""id"": ""90ac1445-44dc-4138-97b2-4148515fa0db"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Heal"",
                    ""type"": ""Button"",
                    ""id"": ""18aa83fd-2463-4529-9ffa-a2a09e842281"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4b3b2104-b0a1-4808-9529-7fe56091468c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""JoystickMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46f42a80-87ed-412c-ac97-6bb965067f97"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""JoystickMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c2c9922-ed04-404d-bfec-495a720dd8c3"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""JoystickMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9920da6e-1d79-4864-ad80-22dbb2463f6e"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""JoystickMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82509977-c4d7-45a2-88bb-219f4a18f26a"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a50142c-fab1-4624-a2fa-8c20d2a91bc9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a8db5c2-58c1-419c-a1c6-a4a97c662da6"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48dc1a33-3eb9-4fde-8c7a-6cf3c62a6d99"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78944983-19dd-49cd-8c87-6fcde7fced82"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e6f2eb2-e44c-4c77-834d-47e30ec65f33"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""243e59fc-8aa1-4588-9dde-4fb946647aec"",
                    ""path"": ""<XInputController>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ebd327e8-ca29-4134-93d6-06a6285f3521"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Skill"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d42ac7e6-f41b-4273-9f45-1b4135b109a4"",
                    ""path"": ""<XInputController>/rightShoulder"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Guard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e830ff9-c8ab-4def-9744-29ffe96908c0"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Guard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bafca74f-dbf0-47cb-bd26-26fa01538b2f"",
                    ""path"": ""<XInputController>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Heal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cfe204c-e384-4312-95fe-df6e9a367955"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Heal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Joystick
        m_Joystick = asset.FindActionMap("Joystick", throwIfNotFound: true);
        m_Joystick_JoystickMove = m_Joystick.FindAction("JoystickMove", throwIfNotFound: true);
        m_Joystick_Jump = m_Joystick.FindAction("Jump", throwIfNotFound: true);
        m_Joystick_Attack = m_Joystick.FindAction("Attack", throwIfNotFound: true);
        m_Joystick_Dash = m_Joystick.FindAction("Dash", throwIfNotFound: true);
        m_Joystick_Skill = m_Joystick.FindAction("Skill", throwIfNotFound: true);
        m_Joystick_Guard = m_Joystick.FindAction("Guard", throwIfNotFound: true);
        m_Joystick_Heal = m_Joystick.FindAction("Heal", throwIfNotFound: true);
    }

    ~@JoystickControls()
    {
        UnityEngine.Debug.Assert(!m_Joystick.enabled, "This will cause a leak and performance issues, JoystickControls.Joystick.Disable() has not been called.");
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Joystick
    private readonly InputActionMap m_Joystick;
    private List<IJoystickActions> m_JoystickActionsCallbackInterfaces = new List<IJoystickActions>();
    private readonly InputAction m_Joystick_JoystickMove;
    private readonly InputAction m_Joystick_Jump;
    private readonly InputAction m_Joystick_Attack;
    private readonly InputAction m_Joystick_Dash;
    private readonly InputAction m_Joystick_Skill;
    private readonly InputAction m_Joystick_Guard;
    private readonly InputAction m_Joystick_Heal;
    public struct JoystickActions
    {
        private @JoystickControls m_Wrapper;
        public JoystickActions(@JoystickControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @JoystickMove => m_Wrapper.m_Joystick_JoystickMove;
        public InputAction @Jump => m_Wrapper.m_Joystick_Jump;
        public InputAction @Attack => m_Wrapper.m_Joystick_Attack;
        public InputAction @Dash => m_Wrapper.m_Joystick_Dash;
        public InputAction @Skill => m_Wrapper.m_Joystick_Skill;
        public InputAction @Guard => m_Wrapper.m_Joystick_Guard;
        public InputAction @Heal => m_Wrapper.m_Joystick_Heal;
        public InputActionMap Get() { return m_Wrapper.m_Joystick; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(JoystickActions set) { return set.Get(); }
        public void AddCallbacks(IJoystickActions instance)
        {
            if (instance == null || m_Wrapper.m_JoystickActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_JoystickActionsCallbackInterfaces.Add(instance);
            @JoystickMove.started += instance.OnJoystickMove;
            @JoystickMove.performed += instance.OnJoystickMove;
            @JoystickMove.canceled += instance.OnJoystickMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Attack.started += instance.OnAttack;
            @Attack.performed += instance.OnAttack;
            @Attack.canceled += instance.OnAttack;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @Skill.started += instance.OnSkill;
            @Skill.performed += instance.OnSkill;
            @Skill.canceled += instance.OnSkill;
            @Guard.started += instance.OnGuard;
            @Guard.performed += instance.OnGuard;
            @Guard.canceled += instance.OnGuard;
            @Heal.started += instance.OnHeal;
            @Heal.performed += instance.OnHeal;
            @Heal.canceled += instance.OnHeal;
        }

        private void UnregisterCallbacks(IJoystickActions instance)
        {
            @JoystickMove.started -= instance.OnJoystickMove;
            @JoystickMove.performed -= instance.OnJoystickMove;
            @JoystickMove.canceled -= instance.OnJoystickMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Attack.started -= instance.OnAttack;
            @Attack.performed -= instance.OnAttack;
            @Attack.canceled -= instance.OnAttack;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @Skill.started -= instance.OnSkill;
            @Skill.performed -= instance.OnSkill;
            @Skill.canceled -= instance.OnSkill;
            @Guard.started -= instance.OnGuard;
            @Guard.performed -= instance.OnGuard;
            @Guard.canceled -= instance.OnGuard;
            @Heal.started -= instance.OnHeal;
            @Heal.performed -= instance.OnHeal;
            @Heal.canceled -= instance.OnHeal;
        }

        public void RemoveCallbacks(IJoystickActions instance)
        {
            if (m_Wrapper.m_JoystickActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IJoystickActions instance)
        {
            foreach (var item in m_Wrapper.m_JoystickActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_JoystickActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public JoystickActions @Joystick => new JoystickActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IJoystickActions
    {
        void OnJoystickMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnSkill(InputAction.CallbackContext context);
        void OnGuard(InputAction.CallbackContext context);
        void OnHeal(InputAction.CallbackContext context);
    }
}
