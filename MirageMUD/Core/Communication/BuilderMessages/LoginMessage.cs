using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Communication.BuilderMessages
{
    /// <summary>
    /// Message received from the client to authenticate
    /// </summary>
    public class LoginMessage : Message
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
    }
}
