using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.Communication.BuilderMessages;
using Mirage.Game.World;
using Mirage.Core.Messaging;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// State handler used to process a Gui client connection until login is
    /// completed.
    /// </summary>
    public class AdvancedLoginHandler : ILoginInputHandler
    {
        private IPlayerRepository _playerRepository;

        public AdvancedLoginHandler(IClient client)
        {
            Client = client;
            _playerRepository = MudFactory.GetObject<IPlayerRepository>();
        }


        #region ILoginInputHandler Members

        public void HandleInput(object input)
        {
            if (input == null)
            {
                Client.Write(new Message(MessageType.Prompt, "negotiation.authentication.NannyChallenge"));
            }
            else if (input is LoginMessage)
            {
                LoginMessage login = (LoginMessage)input;
                Player p = (Player) _playerRepository.Load(login.Login);
                if (p == null || !p.ComparePassword(login.Password))
                {
                    Client.Write(new StringMessage(MessageType.PlayerError, "negotiation.authentication.LoginError", "Invalid Login or password, Please try again"));
                }
                else
                {
                    PlayerFinalizer finalizer = new PlayerFinalizer(Client, p);
                    finalizer.Finalize(false);

                    Client.LoginHandler = null;
                    Client.Write(new StringMessage(MessageType.Confirmation, "negotiation.authentication.Login", "Login Successful"));
                }
            }
        }

        #endregion

        public IClient Client { get; set; }
    }
}
