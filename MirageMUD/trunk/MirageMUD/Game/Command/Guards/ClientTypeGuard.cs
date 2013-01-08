using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.World;
using Mirage.Core.Command.Guards;
using Mirage.Core.Command;

namespace Mirage.Game.Command.Guards
{
    public class ClientTypeGuard : ICommandGuard
    {
        public ClientTypeGuard(IEnumerable<Type> clientTypes)
        {
            if (clientTypes == null)
                throw new ArgumentNullException("clientType");

            this.ClientTypes = clientTypes.ToArray();
        }

        public Type[] ClientTypes { get; private set; }

        public bool IsSatisified(IActor actor)
        {
            if (ClientTypes.Length > 0)
            {
                IPlayer player = actor as IPlayer;
                if (player == null || player.Client == null)
                    return false;

                Type clientType = player.Client.GetType();
                return ClientTypes.Any(t => t.IsAssignableFrom(clientType));
            }
            return true;
        }
    }
}
