using System.Collections;
using System.Collections.Generic;
using Counterweight.CameraSystem;
using Counterweight.Core;
using Counterweight.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Gameplay
{
    // Runs the lose sequence: freeze both players, return them to the active checkpoint, reset puzzle objects, snap the camera.
    public class RespawnManager : MonoBehaviour
    {
        [Header("Players")]
        [SerializeField] private Rigidbody2D scoutBody;
        [SerializeField] private Rigidbody2D heavyBody;

        [Header("Checkpoints")]
        [SerializeField] private Checkpoint startCheckpoint;

        [Header("Camera")]
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private ScreenEdgeWalls edgeWalls;

        [Header("Timing and Input")]
        [SerializeField, Min(0f)] private float respawnDelay = 0.6f;
        [Tooltip("Manual return to the last checkpoint. A safety net against any stuck state.")]
        [SerializeField] private InputActionReference restartAction;

        private readonly List<IResettable> resettables = new List<IResettable>();
        private Checkpoint[] checkpoints;
        private Checkpoint activeCheckpoint;
        private PlayerInputReader scoutInput;
        private PlayerInputReader heavyInput;
        private WaitForSeconds respawnWait;
        private GameManager gameManager;

        private void Awake()
        {
            if (scoutBody == null || heavyBody == null || startCheckpoint == null || cameraFollow == null || edgeWalls == null)
            {
                Debug.LogError($"{name}: RespawnManager has unassigned references.", this);
                enabled = false;
                return;
            }

            scoutInput = scoutBody.GetComponent<PlayerInputReader>();
            heavyInput = heavyBody.GetComponent<PlayerInputReader>();
            respawnWait = new WaitForSeconds(respawnDelay);
        }

        private void OnEnable()
        {
            if (restartAction == null)
                return;

            restartAction.action.performed += OnRestartPressed;
            restartAction.action.Enable();
        }

        private void OnDisable()
        {
            if (restartAction == null)
                return;

            restartAction.action.performed -= OnRestartPressed;
            restartAction.action.Disable();
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError($"{name}: no GameManager in the scene.", this);
                enabled = false;
                return;
            }

            gameManager.RespawnStarted += OnRespawnStarted;

            // One-time scene scans in Start are fine. They never run per frame.
            checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            foreach (Checkpoint checkpoint in checkpoints)
                checkpoint.Reached += OnCheckpointReached;

            MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IResettable resettable)
                    resettables.Add(resettable);
            }

            Activate(startCheckpoint);
        }

        private void OnDestroy()
        {
            if (gameManager != null)
                gameManager.RespawnStarted -= OnRespawnStarted;

            if (checkpoints == null)
                return;

            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint != null)
                    checkpoint.Reached -= OnCheckpointReached;
            }
        }

        private void OnCheckpointReached(Checkpoint checkpoint)
        {
            if (gameManager.State != GameState.Playing)
                return;

            if (checkpoint.Order <= activeCheckpoint.Order)
                return;

            Activate(checkpoint);
        }

        private void Activate(Checkpoint checkpoint)
        {
            // Earlier flags stay raised; only the respawn target moves forward.
            activeCheckpoint = checkpoint;
            activeCheckpoint.SetActiveVisual(true);
        }

        private void OnRestartPressed(InputAction.CallbackContext context)
        {
            if (gameManager != null)
                gameManager.RequestCheckpointRestart();
        }

        private void OnRespawnStarted()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            SetPlayersFrozen(true);
            yield return respawnWait;

            Teleport(scoutBody, activeCheckpoint.ScoutSpawn);
            Teleport(heavyBody, activeCheckpoint.HeavySpawn);

            foreach (IResettable resettable in resettables)
                resettable.ResetState();

            cameraFollow.SnapToTargets();
            edgeWalls.Snap();

            SetPlayersFrozen(false);
            gameManager.CompleteRespawn();
        }

        private void SetPlayersFrozen(bool frozen)
        {
            // simulated = false removes the body from physics entirely: no gravity, no collisions, no triggers.
            scoutBody.simulated = !frozen;
            heavyBody.simulated = !frozen;

            if (scoutInput != null)
                scoutInput.enabled = !frozen;

            if (heavyInput != null)
                heavyInput.enabled = !frozen;
        }

        private static void Teleport(Rigidbody2D body, Transform spawn)
        {
            // Set the transform too, so the camera snap reads the new position immediately instead of the old interpolated one.
            body.position = spawn.position;
            body.transform.position = spawn.position;
            body.linearVelocity = Vector2.zero;
        }
    }
}