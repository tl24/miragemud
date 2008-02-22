using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Util;
using Mirage.Core.Communication;
using Mirage.Core.Command;
//using Mirage.Core.Communication.BuilderMessages;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;
//using System.Configuration;
using Mirage.Core.IO;
using Mirage.Core.Communication.BuilderMessages;
using System.Configuration;
using Mirage.Stock.Data;

namespace Mirage.Stock.IO
{
    /// <summary>
    /// State handler used to process a Gui client connection until login is
    /// completed.
    /// </summary>
    public class GuiLoginHandler : ILoginInputHandler
    {
        private IMudClient _client;
        private IPlayerRepository _playerRepository;

        public GuiLoginHandler(IMudClient client)
        {
            _client = client;
            _playerRepository = MudFactory.GetObject<IPlayerRepository>();
        }


        #region ILoginInputHandler Members

        public void HandleInput(object input)
        {
            if (input == null)
            {
                Client.Write(new Message(MessageType.Prompt, Namespaces.Authentication, "Nanny.Challenge"));
            }
            else if (input is LoginMessage)
            {
                LoginMessage login = (LoginMessage)input;
                Player p = (Player) _playerRepository.Load(login.Login);
                if (p == null || !p.ComparePassword(login.Password))
                {
                    Client.Write(new StringMessage(MessageType.PlayerError, Namespaces.Authentication, "Error.Login", "Invalid Login or password, Please try again"));
                }
                else
                {
                    PlayerFinalizer finalizer = new PlayerFinalizer(Client, p);
                    finalizer.Finalize(false);

                    Client.LoginHandler = null;
                    Client.Write(MudFactory.GetObject<IMessageFactory>().GetMessage("negotiation.authentication.Login"));
                    //Client.Write(new StringMessage(MessageType.Information, Namespaces.Negotiation, "Welcome", "\r\nWelcome to MirageMUD 0.1.  Still in development.\r\n"));
                }
            }
        }

        #endregion

        public IMudClient Client
        {
            get { return this._client; }
            set { this._client = value; }
        }
    }
}
