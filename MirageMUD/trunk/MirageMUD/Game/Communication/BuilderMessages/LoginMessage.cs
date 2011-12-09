
namespace Mirage.Game.Communication.BuilderMessages
{
    /// <summary>
    /// Message received from the client to authenticate
    /// </summary>
    public sealed class LoginMessage : Message
    {
        private string _login;
        private string _password;

        public LoginMessage()
            : base(MessageType.Data, "Login")
        {
        }

        public string Login
        {
            get { return this._login; }
            set { this._login = value; }
        }

        public string Password
        {
            get { return this._password; }
            set { this._password = value; }
        }

        protected override IMessage MakeCopy()
        {
            return new LoginMessage();
        }
    }
}
