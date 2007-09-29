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
        private IClient _client;

        public GuiLoginHandler(IClient client)
        {
            _client = client;
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
                Player p = Player.Load(login.Login);
                if (p == null || !p.ComparePassword(login.Password))
                {
                    Client.Write(new StringMessage(MessageType.PlayerError, Namespaces.Authentication, "Error.Login", "Invalid Login or password, Please try again"));
                }
                else
                {
                    // should put this in an event to be triggered
                    //log_string( $ch->{Name}, "\@", $desc->{HOST}, " has connected." );
                    Client.Logger.Info(string.Format("{0}@{1} has connected.", p.Uri, Client.TcpClient.Client.RemoteEndPoint));
                    MudRepositoryBase globalLists = MudFactory.GetObject<MudRepositoryBase>();
                    globalLists.AddPlayer(p);
                    if (p.Container == null)
                    {
                        Room defaultRoom = (Room)QueryManager.GetInstance().Find(ConfigurationManager.AppSettings["default.room"]);
                        defaultRoom.Add(p);
                    }
                    else
                    {
                        p.Container.Add(p);
                    }

                    //descriptor.writeToBuffer( "Color TesT: " + CLR_TEST + "\r\n");
                    Client.State = ConnectedState.Playing;
                    //Client->WriteToChannel(GLOBAL, $ch->Short . " has entered the game.\r\n",  $desc);	
                    
                    Client.Player = p;
                    p.Client = Client;

                    Client.LoginHandler = null;
                    Client.Write(new StringMessage(MessageType.Confirmation, Namespaces.Authentication, "Login", "Login successful"));
                    Client.Write(new StringMessage(MessageType.Information, Namespaces.Negotiation, "Welcome", "\r\nWelcome to MirageMUD 0.1.  Still in development.\r\n"));
                }
            }
        }

        #endregion

        public IClient Client
        {
            get { return this._client; }
            set { this._client = value; }
        }
    }
}
