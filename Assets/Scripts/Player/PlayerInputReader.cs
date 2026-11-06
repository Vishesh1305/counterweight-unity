using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Reads one player's action map and exposes it as simple values and events.
namespace Counterweight.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference interactAction;

        public float MoveInput { get; private set; }
        public bool JumpHeld { get; private set; }

        public event Action JumpPressed;
        public event Action InteractPressed;

        private void OnEnable()
        {
            if (!HasAllReferences())
            {
                Debug.LogError($"{name}: PlayerInputReader is missing an Input Action Reference.", this);
                enabled = false;
                return;
            }

            moveAction.action.Enable();
            jumpAction.action.Enable();
            interactAction.action.Enable();
        }

        private void OnDisable()
        {
            // Clear held state so a disabled player never keeps walking.
            MoveInput = 0f;
            JumpHeld = false;

            if (!HasAllReferences())
                return;

            moveAction.action.Disable();
            jumpAction.action.Disable();
            interactAction.action.Disable();
        }

        private void Update()
        {
            MoveInput = moveAction.action.ReadValue<float>();
            JumpHeld = jumpAction.action.IsPressed();

            if (jumpAction.action.WasPressedThisFrame())
                JumpPressed?.Invoke();

            if (interactAction.action.WasPressedThisFrame())
                InteractPressed?.Invoke();
        }

        private bool HasAllReferences()
        {
            return moveAction != null && jumpAction != null && interactAction != null;
        }
    }
}