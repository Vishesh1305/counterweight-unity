using Counterweight.Core;
using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ExitDoor : MonoBehaviour
    {
        [Header("Players")]
        [SerializeField] private Rigidbody2D scoutBody;
        [SerializeField] private Rigidbody2D heavyBody;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer bottomRenderer;
        [SerializeField] private SpriteRenderer topRenderer;
        [SerializeField] private Sprite closedBottom;
        [SerializeField] private Sprite closedTop;
        [SerializeField] private Sprite openBottom;
        [SerializeField] private Sprite openTop;

        private Collider2D zone;
        private GameManager gameManager;
        private PlayerInputReader scoutInput;
        private PlayerInputReader heavyInput;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void Awake()
        {
            zone = GetComponent<Collider2D>();

            if (scoutBody == null || heavyBody == null)
            {
                Debug.LogError($"{name}: ExitDoor needs Scout Body and Heavy Body assigned.", this);
                enabled = false;
                return;
            }

            scoutInput = scoutBody.GetComponent<PlayerInputReader>();
            heavyInput = heavyBody.GetComponent<PlayerInputReader>();
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

            gameManager.PickupCountsChanged += UpdateVisual;
            gameManager.Won += OnWon;
            UpdateVisual();
        }

        private void OnDestroy()
        {
            if (gameManager == null)
                return;

            gameManager.PickupCountsChanged -= UpdateVisual;
            gameManager.Won -= OnWon;
        }

        private void FixedUpdate()
        {
            if (gameManager.State != GameState.Playing || !gameManager.AllPickupsCollected)
                return;

            // Measured fresh every step, like the plate, so a respawn teleport can never leave a stale "inside" flag.
            if (zone.OverlapPoint(scoutBody.position) && zone.OverlapPoint(heavyBody.position))
                gameManager.DeclareWin();
        }

        private void OnWon()
        {
            FreezePlayer(scoutBody, scoutInput);
            FreezePlayer(heavyBody, heavyInput);
        }

        private static void FreezePlayer(Rigidbody2D body, PlayerInputReader input)
        {
            body.linearVelocity = Vector2.zero;
            body.simulated = false;

            if (input != null)
                input.enabled = false;
        }

        private void UpdateVisual()
        {
            bool open = gameManager.AllPickupsCollected;
            SetSprite(bottomRenderer, open ? openBottom : closedBottom);
            SetSprite(topRenderer, open ? openTop : closedTop);
        }

        private static void SetSprite(SpriteRenderer renderer, Sprite sprite)
        {
            if (renderer != null && sprite != null)
                renderer.sprite = sprite;
        }
    }
}