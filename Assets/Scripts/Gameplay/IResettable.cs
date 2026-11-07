namespace Counterweight.Gameplay
{
    // Anything that must return to its starting state when the players respawn (crates, levers, platforms, gates).
    public interface IResettable
    {
        void ResetState();
    }
}