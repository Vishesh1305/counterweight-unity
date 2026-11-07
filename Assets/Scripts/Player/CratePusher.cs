using System.Collections.Generic;
using Counterweight.Gameplay;
using UnityEngine;

namespace Counterweight.Player
{
    // Pushes any crate directly in front of the player while it walks into it on the ground. The crate decides whether this role may push it.
    [RequireComponent(typeof(PlayerIdentity))]
    public class CratePusher : MonoBehaviour
    {
        private const int MaxResults = 4;
        private const float InputDeadZone = 0.1f;

        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Collider2D bodyCollider;
        [SerializeField] private LayerMask crateLayers;
        [SerializeField, Min(0f)] private float pushSpeed = 3f;
        [Tooltip("How far in front of the body the probe reaches, in world units.")]
        [SerializeField, Min(0.01f)] private float probeDepth = 0.1f;
        [Tooltip("Probe height as a fraction of body height. Below 1 so a crate under the feet or overhead is never pushed.")]
        [SerializeField, Range(0.1f, 1f)] private float probeHeightRatio = 0.8f;

        private readonly List<Collider2D> results = new List<Collider2D>(MaxResults);
        private ContactFilter2D filter;
        private PlayerIdentity identity;

        private void Awake()
        {
            identity = GetComponent<PlayerIdentity>();

            if (input == null || movement == null || bodyCollider == null)
            {
                Debug.LogError($"{name}: CratePusher needs Input, Movement and Body Collider assigned.", this);
                enabled = false;
                return;
            }

            filter = new ContactFilter2D();
            filter.SetLayerMask(crateLayers);
            filter.useTriggers = false;
        }

        private void FixedUpdate()
        {
            float move = input.MoveInput;
            if (!movement.IsGrounded || Mathf.Abs(move) < InputDeadZone)
                return;

            float direction = Mathf.Sign(move);
            GetProbe(direction, out Vector2 center, out Vector2 size);
            int count = Physics2D.OverlapBox(center, size, 0f, filter, results);

            for (int i = 0; i < count; i++)
            {
                if (Crate.TryGet(results[i], out Crate crate))
                    crate.Push(identity.Role, direction * pushSpeed);
            }
        }

        private void GetProbe(float direction, out Vector2 center, out Vector2 size)
        {
            Bounds bounds = bodyCollider.bounds;
            float edgeX = direction > 0f ? bounds.max.x + probeDepth * 0.5f : bounds.min.x - probeDepth * 0.5f;
            center = new Vector2(edgeX, bounds.center.y);
            size = new Vector2(probeDepth, bounds.size.y * probeHeightRatio);
        }

        private void OnDrawGizmosSelected()
        {
            if (bodyCollider == null)
                return;

            Gizmos.color = new Color(1f, 0.5f, 0f);
            GetProbe(1f, out Vector2 rightCenter, out Vector2 size);
            Gizmos.DrawWireCube(rightCenter, size);
            GetProbe(-1f, out Vector2 leftCenter, out size);
            Gizmos.DrawWireCube(leftCenter, size);
        }
    }
}