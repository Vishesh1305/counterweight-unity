using Counterweight.Core;
using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Gameplay
{
    // A collectible that only one role can pick up. The other player passes straight through it.
    [RequireComponent(typeof(Collider2D))]
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private PickupType type = PickupType.Coin;
        [SerializeField] private PlayerRole collectedBy = PlayerRole.Scout;

        private bool collected;

        public PickupType Type => type;

        private void Reset()
        {
            // Editor convenience: a pickup's collider must be a trigger.
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void Start()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError($"{name}: no GameManager in the scene, so this pickup is not counted.", this);
                return;
            }

            GameManager.Instance.RegisterPickup(type);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collected)
                return;

            // The Rigidbody2D sits on the player root, so this works even if colliders move to child objects later.
            Rigidbody2D body = other.attachedRigidbody;
            if (body == null || !body.TryGetComponent(out PlayerIdentity player))
                return;

            if (player.Role != collectedBy)
                return;

            collected = true;

            if (GameManager.Instance != null)
                GameManager.Instance.CollectPickup(type);

            gameObject.SetActive(false);
        }
    }
}