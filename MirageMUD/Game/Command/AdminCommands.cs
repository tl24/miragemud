using System.Collections;
using System.Linq;
using Mirage.Game.Server;
using Mirage.Game.World;
using Mirage.Core.Command;

namespace Mirage.Game.Command
{
    [RoleRestriction("Admin")]
    public class AdminCommands : CommandDefaults
    {
        [Command]
        public void Shutdown([Actor]IActor actor)
        {
            Logger.Info("Shutdown initiated by " + actor);
            foreach (Player player in World.Players)
            {
                player.ToSelf("shutdown", "The mud is shutting down");
            }
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
            object result = World.ResolveUri(actor, uri);
            if (result == null)
            {
                actor.WriteLine("common.error.notfound", "No object found with uri: " + uri);
            }
            else
            {
                actor.WriteLine("object.uri", DumpObject(result));
            }
        }

        private string DumpObject(object result) {

            string msg = "";
            msg += result.GetType().Name + "\r\n";
            msg += "--------------------------------------\r\n";
            if (result is IDictionary)
            {
                msg += string.Format("{0}: {1}\r\n", "Keys", DumpDictionaryKeys(result as IDictionary));
                msg += string.Format("{0}: {1}\r\n", "Count", ((IDictionary)result).Count);
            }
            else
            {
                var q = from p in result.GetType().GetProperties()
                        let getter = p.GetGetMethod()
                        where getter.GetParameters().Length == 0
                        orderby p.Name
                        select new { Name = p.Name, Value = p.GetGetMethod().Invoke(result, null) };
                foreach (var pv in q)
                {
                    msg += string.Format("{0}: {1}\r\n", pv.Name, DumpValue(pv.Value));
                }
            }
            msg += "\r\n";
            return msg;
        }

        private string DumpValue(object result)
        {
            if (result == null)
                return "";

            if (result is IDictionary)
            {
                IDictionary dict = result as IDictionary;
                return DumpDictionaryKeys(dict);
            }
            return result.ToString();
        }

        private static string DumpDictionaryKeys(IDictionary dict, int max = 5)
        {
            int count = dict.Count;
            string text = string.Join(", ", from object k in dict.Keys.Cast<object>().Take(max)
                                            select k.ToString());
            if (count > max)
                text += "...";
            return text;
        }
    }
}
