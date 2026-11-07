using Counterweight.Core;
using UnityEngine;

namespace Counterweight.Player
{
    // Reports this player's death to the GameManager when it touches anything on a hazard layer.
    public class PlayerHazardDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask hazardLayers;

        private void OnTriggerEnter2D(Collider2D other)
        {
            bool isHazard = (hazardLayers.value & (1 << other.gameObject.layer)) != 0;
            if (!isHazard)
                return;

            if (GameManager.Instance != null)
                GameManager.Instance.ReportPlayerDeath();
        }
    }
}