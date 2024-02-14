using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CustomInputs.Controls;

namespace CustomInputs
{
    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event Action<bool> PrimaryFireEvent;
        public event Action<Vector2> MoveEvent; 
        
        public Vector2 AimPosition { get; private set; }
        
        //Private members
        private Controls m_controls;
        private void OnEnable()
        {
            if (m_controls == null)
            {
                m_controls = new Controls();
                m_controls.Player.SetCallbacks(this);
            }
            
            m_controls.Player.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPrimaryFire(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                PrimaryFireEvent?.Invoke(true);
            }
            else if(context.canceled)
            {
                PrimaryFireEvent?.Invoke(false);
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            AimPosition = context.ReadValue<Vector2>();
        }
    }
}
