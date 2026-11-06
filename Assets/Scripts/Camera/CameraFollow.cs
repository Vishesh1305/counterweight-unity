using UnityEngine;

namespace Counterweight.CameraSystem
{
    // Follows the Scout, but never so far that the partner leaves the screen, and never outside the level.
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [Header("Targets")]
        [Tooltip("The player the camera follows (Scout).")]
        [SerializeField] private Transform followTarget;
        [Tooltip("The other player, who must always stay on screen (Heavy).")]
        [SerializeField] private Transform partnerTarget;

        [Header("Framing")]
        [Tooltip("Minimum gap in world units between either player and the screen edge.")]
        [SerializeField, Min(0f)] private float edgeMargin = 1.5f;
        [Tooltip("Shifts the view relative to the follow target. Positive Y shows more space above.")]
        [SerializeField] private Vector2 followOffset = new Vector2(0f, 2f);
        [SerializeField, Min(0f)] private float smoothTime = 0.15f;

        [Header("Level Bounds (world units)")]
        [SerializeField] private Vector2 levelMin = new Vector2(-15f, -9f);
        [SerializeField] private Vector2 levelMax = new Vector2(15f, 9f);

        private Camera cam;
        private Vector3 smoothVelocity;

        public float HalfWidth => cam.orthographicSize * cam.aspect;
        public float HalfHeight => cam.orthographicSize;

        private void Awake()
        {
            cam = GetComponent<Camera>();

            if (followTarget == null || partnerTarget == null)
            {
                Debug.LogError($"{name}: CameraFollow needs both Follow Target and Partner Target.", this);
                enabled = false;
            }
        }

        private void Start()
        {
            SnapToTargets();
        }

        private void LateUpdate()
        {
            // LateUpdate runs after the players' interpolated positions are final for this frame, so the follow does not jitter.
            Vector3 goal = GetGoalPosition();
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref smoothVelocity, smoothTime);
        }

        // Jumps straight to the framed position with no smoothing. Used at startup and after a respawn.
        public void SnapToTargets()
        {
            if (!enabled)
                return;

            transform.position = GetGoalPosition();
            smoothVelocity = Vector3.zero;
        }

        private Vector3 GetGoalPosition()
        {
            Vector2 follow = followTarget.position;
            Vector2 partner = partnerTarget.position;

            float x = KeepBothInView(follow.x + followOffset.x, follow.x, partner.x, HalfWidth);
            float y = KeepBothInView(follow.y + followOffset.y, follow.y, partner.y, HalfHeight);

            x = ClampToLevel(x, levelMin.x, levelMax.x, HalfWidth);
            y = ClampToLevel(y, levelMin.y, levelMax.y, HalfHeight);

            return new Vector3(x, y, transform.position.z);
        }

        // Limits the camera centre on one axis so both players stay at least edgeMargin inside the view.
        private float KeepBothInView(float desired, float a, float b, float halfExtent)
        {
            float reach = halfExtent - edgeMargin;
            float lowestCentre = Mathf.Max(a, b) - reach;
            float highestCentre = Mathf.Min(a, b) + reach;

            // Both cannot fit: favour the lower (or left) player. Vertically this keeps the player who could fall in view.
            if (lowestCentre > highestCentre)
                return highestCentre;

            return Mathf.Clamp(desired, lowestCentre, highestCentre);
        }

        private static float ClampToLevel(float centre, float min, float max, float halfExtent)
        {
            float lowest = min + halfExtent;
            float highest = max - halfExtent;

            // Level smaller than the view on this axis: centre on the level.
            if (lowest > highest)
                return (min + max) * 0.5f;

            return Mathf.Clamp(centre, lowest, highest);
        }

        private void OnValidate()
        {
            levelMax = Vector2.Max(levelMax, levelMin);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector2 centre = (levelMin + levelMax) * 0.5f;
            Vector2 size = levelMax - levelMin;
            Gizmos.DrawWireCube(centre, size);
        }
    }
}