using UnityEngine;

namespace Counterweight.CameraSystem
{
    // Keeps two invisible walls on the screen's left and right edges so neither player can leave the view.
    public class ScreenEdgeWalls : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private BoxCollider2D leftWall;
        [SerializeField] private BoxCollider2D rightWall;
        [SerializeField, Min(0.1f)] private float wallThickness = 1f;
        [Tooltip("Extra wall height above and below the view, so a jump cannot clear the wall.")]
        [SerializeField, Min(0f)] private float extraHeight = 10f;

        private Rigidbody2D leftBody;
        private Rigidbody2D rightBody;
        private bool needsSnap = true;

        private void Awake()
        {
            if (targetCamera == null || leftWall == null || rightWall == null)
            {
                Debug.LogError($"{name}: ScreenEdgeWalls needs Target Camera, Left Wall and Right Wall assigned.", this);
                enabled = false;
                return;
            }

            leftBody = leftWall.attachedRigidbody;
            rightBody = rightWall.attachedRigidbody;

            if (leftBody == null || rightBody == null)
            {
                Debug.LogError($"{name}: each wall needs a Kinematic Rigidbody 2D.", this);
                enabled = false;
                return;
            }

            Vector2 size = new Vector2(wallThickness, targetCamera.orthographicSize * 2f + extraHeight * 2f);
            leftWall.size = size;
            rightWall.size = size;
            leftWall.offset = Vector2.zero;
            rightWall.offset = Vector2.zero;
        }

        // Call after the camera jumps so the walls teleport instead of sweeping across the level.
        public void Snap()
        {
            needsSnap = true;
        }

        private void FixedUpdate()
        {
            Vector2 centre = targetCamera.transform.position;
            float halfWidth = targetCamera.orthographicSize * targetCamera.aspect;
            float distanceFromCentre = halfWidth + wallThickness * 0.5f;

            Vector2 leftTarget = new Vector2(centre.x - distanceFromCentre, centre.y);
            Vector2 rightTarget = new Vector2(centre.x + distanceFromCentre, centre.y);

            if (needsSnap)
            {
                // Setting position directly teleports. MovePosition would sweep and shove any player in the path.
                leftBody.position = leftTarget;
                rightBody.position = rightTarget;
                needsSnap = false;
                return;
            }

            leftBody.MovePosition(leftTarget);
            rightBody.MovePosition(rightTarget);
        }
    }
}