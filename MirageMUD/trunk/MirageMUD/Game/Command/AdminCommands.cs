using Castle.Core.Logging;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;
using System.Linq;

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

        [Command]
        public void DumpUri([Actor] IActor actor)
        {
            DumpUri(actor, ".");
        }

        [Command]
        public void DumpUri([Actor] IActor actor, string uri)
        {
            IQueryManager queryMgr = MudFactory.GetObject<IQueryManager>();
            object result = queryMgr.Find(uri);
            if (result == null)
            {
                actor.Write("common.error.notfound", "No object found with uri: " + uri);
            }
            else
            {
                actor.Write("object.uri", DumpObject(result));
            }
        }

        private string DumpObject(object result) {
            var q = from p in result.GetType().GetProperties()
                    orderby p.Name
                    select new { Name = p.Name, Value = p.GetGetMethod().Invoke(result, null) };
            string msg = "";
            foreach (var pv in q)
            {
                msg += string.Format("{0}: {1}\r\n", pv.Name, pv.Value);
            }
            return msg;
        }
    }
}
