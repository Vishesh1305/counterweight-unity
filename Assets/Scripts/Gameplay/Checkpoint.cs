using System;
using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Gameplay
{
    // A respawn point for both players. Reports when either player touches it; RespawnManager decides whether it activates.
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        [Tooltip("Checkpoints activate only in increasing order, so walking back never moves the respawn point backwards.")]
        [SerializeField, Min(0)] private int order;
        [SerializeField] private Transform scoutSpawn;
        [SerializeField] private Transform heavySpawn;

        [Header("Visuals (optional)")]
        [SerializeField] private SpriteRenderer flagRenderer;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private Sprite activeSprite;

        public int Order => order;
        public Transform ScoutSpawn => scoutSpawn;
        public Transform HeavySpawn => heavySpawn;

        public event Action<Checkpoint> Reached;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void Awake()
        {
            if (scoutSpawn == null || heavySpawn == null)
                Debug.LogError($"{name}: Checkpoint needs both spawn points assigned.", this);

            SetActiveVisual(false);
        }

        public void SetActiveVisual(bool active)
        {
            if (flagRenderer == null)
                return;

            Sprite sprite = active ? activeSprite : inactiveSprite;
            if (sprite != null)
                flagRenderer.sprite = sprite;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Rigidbody2D body = other.attachedRigidbody;
            if (body != null && body.TryGetComponent(out PlayerIdentity _))
                Reached?.Invoke(this);
        }
    }
}