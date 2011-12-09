
namespace Mirage.Game
{
    /// <summary>
    /// A module that runs on startup.  These should be registered in the config file
    /// </summary>
    public interface IInitializer
    {
        string Name { get; }
        void Execute();
    }    
}
