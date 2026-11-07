using UnityEngine;

namespace Counterweight.Player
{
    // Tags a player root with its role so gameplay objects can tell the two players apart.
    public class PlayerIdentity : MonoBehaviour
    {
        [SerializeField] private PlayerRole role;

        public PlayerRole Role => role;
    }
}