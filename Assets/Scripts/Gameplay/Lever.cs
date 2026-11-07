using Counterweight.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Counterweight.Gameplay
{
    // A switch only one role can flip (Scout by default). Toggles its targets through a UnityEvent<bool>.
    [RequireComponent(typeof(Collider2D))]
    public class Lever : MonoBehaviour, IInteractable, IResettable
    {
        [SerializeField] private PlayerRole usableBy = PlayerRole.Scout;
        [SerializeField] private bool startOn;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;

        [Header("Output")]
        [SerializeField] private UnityEvent<bool> onToggled;

        private bool isOn;

        public bool IsOn => isOn;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void Awake()
        {
            isOn = startOn;
            UpdateVisual();
        }

        public void Interact(PlayerRole role)
        {
            if (role != usableBy)
                return;

            isOn = !isOn;
            UpdateVisual();
            onToggled?.Invoke(isOn);
        }

        public void ResetState()
        {
            // Targets reset themselves through IResettable, so no event is fired here; nothing gets toggled twice.
            isOn = startOn;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null)
                return;

            Sprite sprite = isOn ? onSprite : offSprite;
            if (sprite != null)
                spriteRenderer.sprite = sprite;
        }
    }
}