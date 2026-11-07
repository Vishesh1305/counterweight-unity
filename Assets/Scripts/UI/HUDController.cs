using Counterweight.Core;
using TMPro;
using UnityEngine;

namespace Counterweight.UI
{
    // Shows both players' pickup progress. Updates only when the GameManager reports a change.
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinCountText;
        [SerializeField] private TMP_Text gemCountText;

        private GameManager gameManager;

        private void Start()
        {
            // Start, not OnEnable: every Awake has run by now, so GameManager.Instance is guaranteed to exist.
            gameManager = GameManager.Instance;

            if (gameManager == null || coinCountText == null || gemCountText == null)
            {
                Debug.LogError($"{name}: HUDController needs a GameManager in the scene and both count texts assigned.", this);
                enabled = false;
                return;
            }

            gameManager.PickupCountsChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (gameManager != null)
                gameManager.PickupCountsChanged -= Refresh;
        }

        private void Refresh()
        {
            coinCountText.text = $"{gameManager.CoinsCollected} / {gameManager.CoinsTotal}";
            gemCountText.text = $"{gameManager.GemsCollected} / {gameManager.GemsTotal}";
        }
    }
}
