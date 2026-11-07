using System.Collections.Generic;
using Counterweight.Gameplay;
using UnityEngine;

namespace Counterweight.Player
{
    // When the interact key is pressed, uses the nearest interactable in reach on behalf of this player's role.
    [RequireComponent(typeof(PlayerIdentity))]
    public class PlayerInteractor : MonoBehaviour
    {
        private const int MaxResults = 4;

        [SerializeField] private PlayerInputReader input;
        [SerializeField, Min(0.1f)] private float reach = 0.8f;
        [SerializeField] private LayerMask interactableLayers;

        private readonly List<Collider2D> results = new List<Collider2D>(MaxResults);
        private ContactFilter2D filter;
        private PlayerIdentity identity;

        private void Awake()
        {
            identity = GetComponent<PlayerIdentity>();

            if (input == null)
            {
                Debug.LogError($"{name}: PlayerInteractor needs Input assigned.", this);
                enabled = false;
                return;
            }

            filter = new ContactFilter2D();
            filter.SetLayerMask(interactableLayers);
            filter.useTriggers = true;
        }

        private void OnEnable()
        {
            if (input != null)
                input.InteractPressed += TryInteract;
        }

        private void OnDisable()
        {
            if (input != null)
                input.InteractPressed -= TryInteract;
        }

        private void TryInteract()
        {
            // Runs only on a key press, never per frame, so the component lookup below is cheap.
            int count = Physics2D.OverlapCircle(transform.position, reach, filter, results);

            IInteractable nearest = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!results[i].TryGetComponent(out IInteractable interactable))
                    continue;

                float distance = ((Vector2)results[i].transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = interactable;
                }
            }

            if (nearest != null)
                nearest.Interact(identity.Role);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, reach);
        }
    }
}