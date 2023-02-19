//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/GameInput.inputactions
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

namespace MyInput
{
    public partial class @DebugInputMap : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @DebugInputMap()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""4899edca-3177-4b1f-97ad-5e51858dd314"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""ce0e71ba-529a-41bb-972a-0a6b43595811"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7acd69b6-f658-4356-ad26-54a19186eac9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Punch"",
                    ""type"": ""Button"",
                    ""id"": ""8bfed039-217d-415d-bcd2-ad50a8094fb0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill"",
                    ""type"": ""Button"",
                    ""id"": ""df407c11-82c8-4a7b-a28f-c81a0dca0d00"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fix"",
                    ""type"": ""Button"",
                    ""id"": ""b8b934e2-c94e-4364-ad03-0d020a99ff48"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Player1"",
                    ""type"": ""Button"",
                    ""id"": ""ed3e50e8-6244-4e12-857c-59d70820006e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Player2"",
                    ""type"": ""Button"",
                    ""id"": ""524289de-a444-4874-8adb-9267910bf80a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Player3"",
                    ""type"": ""Button"",
                    ""id"": ""8f9fb6c2-075a-40d0-8ac1-34bc69d93fff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Player4"",
                    ""type"": ""Button"",
                    ""id"": ""fc428a50-baa7-4b9d-82d8-a249c437a75a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PlayerSelect"",
                    ""type"": ""Button"",
                    ""id"": ""11dc477a-661f-4bdb-a3ad-ae753ee9f0e9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""0346b61c-035a-497e-a0db-448e97b0979c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ceddfb67-56dd-444e-91a4-9fc012833d9e"",
                    ""path"": ""<Keyboard>/#(A)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4d80e044-39f8-473a-8b07-7f36a06c768b"",
                    ""path"": ""<Keyboard>/#(D)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d053f7e6-362b-4350-adda-303eaa09674c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87617b60-de4a-4c41-b9c8-ba5ce1599e9d"",
                    ""path"": ""<Keyboard>/#(E)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a2851fa8-5b52-485d-b2c8-b1f0d274e71f"",
                    ""path"": ""<Keyboard>/#(Q)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""98a4fa7c-04b7-404f-a040-b03165de642e"",
                    ""path"": ""<Keyboard>/#(1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Player1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce13a357-3dbe-4ea1-aff8-b8fd101617f1"",
                    ""path"": ""<Keyboard>/#(4)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Player4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0384ec9-b5c0-42c8-b46e-8bb1b527bdca"",
                    ""path"": ""<Keyboard>/#(2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Player2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8b41166-3b08-4e8b-97ae-661692ad1eac"",
                    ""path"": ""<Keyboard>/#(3)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Player3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7dac8ae5-06f5-4dd7-9d4b-1904753d3132"",
                    ""path"": ""<Keyboard>/#(W)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fix"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a4c2086-3919-43ff-a681-81abe252bffb"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""TalkEnter"",
            ""id"": ""9df0b4bb-d696-40eb-950f-c58b09507372"",
            ""actions"": [
                {
                    ""name"": ""Enter"",
                    ""type"": ""Button"",
                    ""id"": ""5df14a37-4251-48a0-9b51-4d4b168e7cfa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2e923dea-0ba9-4c8c-a810-2b09bfd9a3ea"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83219e90-4350-41cd-a946-5969b246da57"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            m_Player_Punch = m_Player.FindAction("Punch", throwIfNotFound: true);
            m_Player_Skill = m_Player.FindAction("Skill", throwIfNotFound: true);
            m_Player_Fix = m_Player.FindAction("Fix", throwIfNotFound: true);
            m_Player_Player1 = m_Player.FindAction("Player1", throwIfNotFound: true);
            m_Player_Player2 = m_Player.FindAction("Player2", throwIfNotFound: true);
            m_Player_Player3 = m_Player.FindAction("Player3", throwIfNotFound: true);
            m_Player_Player4 = m_Player.FindAction("Player4", throwIfNotFound: true);
            m_Player_PlayerSelect = m_Player.FindAction("PlayerSelect", throwIfNotFound: true);
            // TalkEnter
            m_TalkEnter = asset.FindActionMap("TalkEnter", throwIfNotFound: true);
            m_TalkEnter_Enter = m_TalkEnter.FindAction("Enter", throwIfNotFound: true);
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

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Jump;
        private readonly InputAction m_Player_Punch;
        private readonly InputAction m_Player_Skill;
        private readonly InputAction m_Player_Fix;
        private readonly InputAction m_Player_Player1;
        private readonly InputAction m_Player_Player2;
        private readonly InputAction m_Player_Player3;
        private readonly InputAction m_Player_Player4;
        private readonly InputAction m_Player_PlayerSelect;
        public struct PlayerActions
        {
            private @DebugInputMap m_Wrapper;
            public PlayerActions(@DebugInputMap wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputAction @Punch => m_Wrapper.m_Player_Punch;
            public InputAction @Skill => m_Wrapper.m_Player_Skill;
            public InputAction @Fix => m_Wrapper.m_Player_Fix;
            public InputAction @Player1 => m_Wrapper.m_Player_Player1;
            public InputAction @Player2 => m_Wrapper.m_Player_Player2;
            public InputAction @Player3 => m_Wrapper.m_Player_Player3;
            public InputAction @Player4 => m_Wrapper.m_Player_Player4;
            public InputAction @PlayerSelect => m_Wrapper.m_Player_PlayerSelect;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Punch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPunch;
                    @Punch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPunch;
                    @Punch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPunch;
                    @Skill.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill;
                    @Skill.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill;
                    @Skill.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill;
                    @Fix.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFix;
                    @Fix.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFix;
                    @Fix.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFix;
                    @Player1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer1;
                    @Player1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer1;
                    @Player1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer1;
                    @Player2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer2;
                    @Player2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer2;
                    @Player2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer2;
                    @Player3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer3;
                    @Player3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer3;
                    @Player3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer3;
                    @Player4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer4;
                    @Player4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer4;
                    @Player4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayer4;
                    @PlayerSelect.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayerSelect;
                    @PlayerSelect.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayerSelect;
                    @PlayerSelect.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlayerSelect;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Punch.started += instance.OnPunch;
                    @Punch.performed += instance.OnPunch;
                    @Punch.canceled += instance.OnPunch;
                    @Skill.started += instance.OnSkill;
                    @Skill.performed += instance.OnSkill;
                    @Skill.canceled += instance.OnSkill;
                    @Fix.started += instance.OnFix;
                    @Fix.performed += instance.OnFix;
                    @Fix.canceled += instance.OnFix;
                    @Player1.started += instance.OnPlayer1;
                    @Player1.performed += instance.OnPlayer1;
                    @Player1.canceled += instance.OnPlayer1;
                    @Player2.started += instance.OnPlayer2;
                    @Player2.performed += instance.OnPlayer2;
                    @Player2.canceled += instance.OnPlayer2;
                    @Player3.started += instance.OnPlayer3;
                    @Player3.performed += instance.OnPlayer3;
                    @Player3.canceled += instance.OnPlayer3;
                    @Player4.started += instance.OnPlayer4;
                    @Player4.performed += instance.OnPlayer4;
                    @Player4.canceled += instance.OnPlayer4;
                    @PlayerSelect.started += instance.OnPlayerSelect;
                    @PlayerSelect.performed += instance.OnPlayerSelect;
                    @PlayerSelect.canceled += instance.OnPlayerSelect;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // TalkEnter
        private readonly InputActionMap m_TalkEnter;
        private ITalkEnterActions m_TalkEnterActionsCallbackInterface;
        private readonly InputAction m_TalkEnter_Enter;
        public struct TalkEnterActions
        {
            private @DebugInputMap m_Wrapper;
            public TalkEnterActions(@DebugInputMap wrapper) { m_Wrapper = wrapper; }
            public InputAction @Enter => m_Wrapper.m_TalkEnter_Enter;
            public InputActionMap Get() { return m_Wrapper.m_TalkEnter; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(TalkEnterActions set) { return set.Get(); }
            public void SetCallbacks(ITalkEnterActions instance)
            {
                if (m_Wrapper.m_TalkEnterActionsCallbackInterface != null)
                {
                    @Enter.started -= m_Wrapper.m_TalkEnterActionsCallbackInterface.OnEnter;
                    @Enter.performed -= m_Wrapper.m_TalkEnterActionsCallbackInterface.OnEnter;
                    @Enter.canceled -= m_Wrapper.m_TalkEnterActionsCallbackInterface.OnEnter;
                }
                m_Wrapper.m_TalkEnterActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Enter.started += instance.OnEnter;
                    @Enter.performed += instance.OnEnter;
                    @Enter.canceled += instance.OnEnter;
                }
            }
        }
        public TalkEnterActions @TalkEnter => new TalkEnterActions(this);
        public interface IPlayerActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnPunch(InputAction.CallbackContext context);
            void OnSkill(InputAction.CallbackContext context);
            void OnFix(InputAction.CallbackContext context);
            void OnPlayer1(InputAction.CallbackContext context);
            void OnPlayer2(InputAction.CallbackContext context);
            void OnPlayer3(InputAction.CallbackContext context);
            void OnPlayer4(InputAction.CallbackContext context);
            void OnPlayerSelect(InputAction.CallbackContext context);
        }
        public interface ITalkEnterActions
        {
            void OnEnter(InputAction.CallbackContext context);
        }
    }
}
