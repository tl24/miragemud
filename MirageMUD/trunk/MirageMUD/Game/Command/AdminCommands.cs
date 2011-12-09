using Castle.Core.Logging;
using Mirage.Game.Communication;
using Mirage.Game.World;

namespace Mirage.Game.Command
{
    [CommandDefaults(Roles="Admin")]
    public class AdminCommands
    {
        private ILogger logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        [Command]
        public void Shutdown([Actor]IActor actor)
        {
            Logger.Info("Shutdown initiated by " + actor);
            foreach (IPlayer player in MudFactory.GetObject<IPlayerRepository>())
                player.Write(MudFactory.GetObject<IMessageFactory>().GetMessage("shutdown", "The mud is shutting down"));
            MudFactory.GetObject<MirageServer>().Shutdown = true;
        }
    }
}
