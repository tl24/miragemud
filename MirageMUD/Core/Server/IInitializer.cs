
namespace Mirage.Core.Server
{
    /// <summary>
    /// A module that runs on startup.  These should be registered in the config file
    /// </summary>
    public interface IInitializer
    {
        void Execute();
    }    
}
