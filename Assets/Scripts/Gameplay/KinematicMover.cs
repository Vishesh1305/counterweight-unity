using UnityEngine;

namespace Counterweight.Gameplay
{
    // Moves a kinematic body between its start position and an offset. Drives gates, barriers and moving platforms.
    [RequireComponent(typeof(Rigidbody2D))]
    public class KinematicMover : MonoBehaviour, IResettable
    {
        public enum MoveMode
        {
            Toggle,   // Active: move to the offset and stay there. Inactive: return to start.
            PingPong  // Active: travel back and forth. Inactive: return to start.
        }

        private const float ArrivalTolerance = 0.001f;

        [SerializeField] private MoveMode mode = MoveMode.Toggle;
        [Tooltip("Where the body moves to, relative to its start position, in world units.")]
        [SerializeField] private Vector2 moveOffset = new Vector2(0f, -3f);
        [SerializeField, Min(0.01f)] private float speed = 4f;
        [SerializeField] private bool startActive;

        private Rigidbody2D body;
        private Vector2 startPosition;
        private bool isActive;
        private bool headingToOffset;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            startPosition = body.position;
            isActive = startActive;
            headingToOffset = startActive;
        }

        // Connected to plates and levers through a UnityEvent<bool> in the Inspector.
        public void SetActivated(bool active)
        {
            isActive = active;
            headingToOffset = active;
        }

        public void ResetState()
        {
            isActive = startActive;
            headingToOffset = startActive;
            body.position = startPosition;
            transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
            body.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            Vector2 target = headingToOffset ? startPosition + moveOffset : startPosition;
            Vector2 toTarget = target - body.position;

            if (toTarget.sqrMagnitude <= ArrivalTolerance * ArrivalTolerance)
            {
                body.linearVelocity = Vector2.zero;

                if (mode == MoveMode.PingPong && isActive)
                    headingToOffset = !headingToOffset;

                return;
            }

            // Moved by velocity, not MovePosition, so riders can read this body's linearVelocity and be carried (Step 6.2).
            float maxStep = speed * Time.fixedDeltaTime;
            body.linearVelocity = toTarget.magnitude <= maxStep
                ? toTarget / Time.fixedDeltaTime
                : toTarget.normalized * speed;
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 origin = Application.isPlaying ? startPosition : (Vector2)transform.position;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(origin, origin + moveOffset);
            Gizmos.DrawWireSphere(origin + moveOffset, 0.2f);
        }
    }
}