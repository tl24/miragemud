
namespace Mirage.Game.Command
{
    /// <summary>
    /// Handler interface for handling initial input for a connection until
    /// authentication occurs
    /// </summary>
    public interface ILoginInputHandler
    {
        void HandleInput(object input);
    }
}
