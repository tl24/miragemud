using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Util;
using Mirage.Communication;
using Mirage.Command;
using Mirage.Communication.BuilderMessages;
using Mirage.Data;
using Mirage.Data.Query;
using System.Configuration;

namespace Mirage.IO
{
    public class BuilderLoginStateHandler : ILoginInputHandler
    {
        private IClient _client;

        public BuilderLoginStateHandler(IClient client)
        {
            _client = client;
        }


        #region ILoginInputHandler Members

        public void HandleInput(object input)
        {
            if (input == null)
            {
                Client.Write(new Message(MessageType.Prompt, "Nanny.Challenge"));
            }
            else if (input is LoginMessage)
            {
                LoginMessage login = (LoginMessage)input;
                Player p = Player.Load(login.Login);
                if (p == null)
                {
                    Client.Write(new ErrorMessage("Error.Login", "Invalid Login, Please try again"));
                }
                else if (p.ComparePassword(login.Password))
                {
                    // should put this in an event to be triggered
                    //log_string( $ch->{Name}, "\@", $desc->{HOST}, " has connected." );
                    GlobalLists globalLists = GlobalLists.GetInstance();
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
                    Client.Write(new StringMessage(MessageType.Confirmation, "Login", "Login successful"));
                    Client.Write(new StringMessage(MessageType.Information, "Welcome", "\r\nWelcome to MirageMUD 0.1.  Still in development.\r\n"));
                }
                else
                {
                    Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.WrongPassword", "\r\nWrong password.\r\n"));
                    Client.Write(new Message(MessageType.Prompt, "Nanny.Challenge"));
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
