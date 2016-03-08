using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Security;

namespace SampleMud
{
    public class Player : IActor
    {
        private MudPrincipal _principal;

        public Player(string name)
        {
            this.Level = 1;
            this.Name = name;
        }
        public int Level { get; set; }

        public string Name { get; set; }

        public TextClient Client { get; set; }

        public System.Security.Principal.IPrincipal Principal
        {
            get
            {
                return _principal ?? (_principal = new MudPrincipal(new MudIdentity(Name)));
            }
        }


        public void Write(object sender, Mirage.Core.Messaging.IMessage message)
        {
            Client.Write(message);
        }

        public void Write(Mirage.Core.Messaging.IMessage message)
        {
            Write(null, message);
        }

    }
}
