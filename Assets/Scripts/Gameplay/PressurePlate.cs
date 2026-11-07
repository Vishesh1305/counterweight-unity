using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Counterweight.Gameplay
{
    // Pressed while the total mass resting on it reaches the threshold: Heavy or a crate, never Scout alone.
    public class PressurePlate : MonoBehaviour, IResettable
    {
        private const int MaxResults = 8;

        [Header("Detection")]
        [SerializeField] private Vector2 detectionSize = new Vector2(0.8f, 0.3f);
        [SerializeField] private Vector2 detectionOffset = new Vector2(0f, -0.35f);
        [SerializeField] private LayerMask detectedLayers;
        [Tooltip("Total mass needed to press the plate. Scout is 1, Heavy and crates are 3.")]
        [SerializeField, Min(0f)] private float requiredMass = 2.5f;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite releasedSprite;
        [SerializeField] private Sprite pressedSprite;

        [Header("Output")]
        [SerializeField] private UnityEvent<bool> onPressedChanged;

        private readonly List<Collider2D> results = new List<Collider2D>(MaxResults);
        private readonly List<Rigidbody2D> countedBodies = new List<Rigidbody2D>(MaxResults);
        private ContactFilter2D filter;
        private bool isPressed;

        public bool IsPressed => isPressed;

        private void Awake()
        {
            filter = new ContactFilter2D();
            filter.SetLayerMask(detectedLayers);
            filter.useTriggers = false;
            UpdateVisual();
        }

        private void FixedUpdate()
        {
            // Measured fresh every physics step instead of counting enter/exit events, so a missed exit can never leave it stuck.
            bool pressedNow = MeasureMass() >= requiredMass;
            if (pressedNow == isPressed)
                return;

            isPressed = pressedNow;
            UpdateVisual();
            onPressedChanged?.Invoke(isPressed);
        }

        // After a respawn the plate forgets its state, so the next physics step re-announces it (for example, a crate reset onto it).
        public void ResetState()
        {
            isPressed = false;
            UpdateVisual();
        }

        private float MeasureMass()
        {
            Vector2 centre = (Vector2)transform.position + detectionOffset;
            int count = Physics2D.OverlapBox(centre, detectionSize, 0f, filter, results);

            countedBodies.Clear();
            float totalMass = 0f;

            for (int i = 0; i < count; i++)
            {
                Rigidbody2D body = results[i].attachedRigidbody;

                // A body with several colliders is counted once.
                if (body == null || countedBodies.Contains(body))
                    continue;

                countedBodies.Add(body);
                totalMass += body.mass;
            }

            return totalMass;
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null)
                return;

            Sprite sprite = isPressed ? pressedSprite : releasedSprite;
            if (sprite != null)
                spriteRenderer.sprite = sprite;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector2)transform.position + detectionOffset, detectionSize);
        }
    }
}