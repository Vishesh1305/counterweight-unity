using Counterweight.Player;

namespace Counterweight.Gameplay
{
    // Something a player can use with the interact key. The object decides which roles it accepts.
    public interface IInteractable
    {
        void Interact(PlayerRole role);
    }
}