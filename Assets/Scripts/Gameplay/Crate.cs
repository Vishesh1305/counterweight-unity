using System.Collections.Generic;
using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Gameplay
{
    // A box only one role (Heavy by default) can push. Its X position is locked whenever no allowed player is pushing it.
    [DefaultExecutionOrder(100)] // Runs after every CratePusher, so it always sees this physics step's pushes.
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Crate : MonoBehaviour, IResettable
    {
        private const RigidbodyConstraints2D LockedConstraints =
            RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        private const RigidbodyConstraints2D FreeConstraints = RigidbodyConstraints2D.FreezeRotation;

        // Collider to crate lookup, so pushers never call GetComponent during physics steps.
        private static readonly Dictionary<Collider2D, Crate> Registry = new Dictionary<Collider2D, Crate>();

        [SerializeField] private PlayerRole pushableBy = PlayerRole.Heavy;
        [Tooltip("If the crate falls this far below its start (into a pit), it returns to its start so the puzzle stays solvable.")]
        [SerializeField, Min(1f)] private float fallResetDepth = 15f;

        private Rigidbody2D body;
        private Collider2D crateCollider;
        private Vector2 startPosition;
        private bool pushedThisStep;
        private float pushVelocityX;

        public static bool TryGet(Collider2D collider, out Crate crate)
        {
            return Registry.TryGetValue(collider, out crate);
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            crateCollider = GetComponent<Collider2D>();
            startPosition = body.position;
            SetLocked(true);
        }

        private void OnEnable()
        {
            Registry[crateCollider] = this;
        }

        private void OnDisable()
        {
            Registry.Remove(crateCollider);
        }

        // Called by a CratePusher during its FixedUpdate. Pushes from the wrong role are ignored.
        public void Push(PlayerRole role, float velocityX)
        {
            if (role != pushableBy)
                return;

            pushedThisStep = true;
            pushVelocityX = velocityX;
        }

        public void ResetState()
        {
            pushedThisStep = false;
            SetLocked(true);
            body.position = startPosition;
            transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
            body.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (body.position.y < startPosition.y - fallResetDepth)
            {
                ResetState();
                return;
            }

            Vector2 velocity = body.linearVelocity;
            SetLocked(!pushedThisStep);
            velocity.x = pushedThisStep ? pushVelocityX : 0f;
            body.linearVelocity = velocity;

            // Pushes count for one step only; the pusher must keep pushing every step.
            pushedThisStep = false;
        }

        private void SetLocked(bool locked)
        {
            RigidbodyConstraints2D wanted = locked ? LockedConstraints : FreeConstraints;
            if (body.constraints != wanted)
                body.constraints = wanted;
        }
    }
}