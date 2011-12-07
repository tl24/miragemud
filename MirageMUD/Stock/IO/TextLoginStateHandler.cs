using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Communication;
using Mirage.Core.Data.Query;
using System.Configuration;
using System.Text.RegularExpressions;
using Mirage.Core.Util;
using Mirage.Core.IO;
using System.Security.Principal;

namespace Mirage.Stock.IO
{
    /// <summary>
    /// StateHandler class that controls Login and player creation for the text client.
    /// </summary>
    public class TextLoginStateHandler : AbstractStateMachine
    {
        private bool _failed;
        private bool _echoOn;
        private IMessageFactory _messageFactory;
        private IPlayerRepository _playerRepository;
        private IRaceRepository _raceRepository;

        public TextLoginStateHandler(IConnectionAdapter client)
            : base(client)
        {
            _failed = false;
            _echoOn = true;
            _playerRepository = MudFactory.GetObject<IPlayerRepository>();
            _raceRepository = MudFactory.GetObject<IRaceRepository>();
        }
        
        public IMessageFactory MessageFactory
        {
            get
            {
                if (_messageFactory == null)
                {
                    _messageFactory = MudFactory.GetObject<IMessageFactory>();
                }
                return this._messageFactory;
            }
            set { this._messageFactory = value; }
        }

        protected override void DetermineNextState()
        {
            if (!Contains("name"))
                Require(MessageFactory.GetMessage("negotiation.authentication.EnterName"), new ValidateDelegate(this.ValidateName));
            else if (GetValue<bool>("isNew") == true) {
                if (!Contains("confirmName")) {
                    IMessage rm = MessageFactory.GetMessage("negotiation.authentication.ConfirmNewName");
                    rm["player"] = GetValue<Player>("player").Title;
                    Require(rm, new ValidateDelegate(this.ConfirmName));
                }
                else if (!Contains("password"))
                {
                    MultipartMessage message = new MultipartMessage("negotiation.authentication.password");
                    message.Parts.Add(MessageFactory.GetMessage("negotiation.authentication.NewplayerPassword"));
                    ((IMessage)message.Parts[0])["player"] = GetValue<Player>("player").Title;
                    message.Parts.Add(MessageFactory.GetMessage("system.EchoOff"));

                    Require(message, new ValidateDelegate(this.ValidateNewPassword));
                }
                else if (!Contains("confirmPassword"))
                {
                    MultipartMessage message = new MultipartMessage("negotiation.authentication.confirmPassword");
                    message.Parts.Add(MessageFactory.GetMessage("negotiation.authentication.ConfirmPassword"));
                    message.Parts.Add(MessageFactory.GetMessage("system.EchoOff"));

                    Require(message, new ValidateDelegate(this.ConfirmPassword));
                }
                else
                {
                    Finished = true;
                }
            } else {
                if (!Contains("password"))
                {
                    List<IMessage> message = new List<IMessage>();
                    //MultipartMessage message = new MultipartMessage(MessageType.Multiple, "Nanny.Password");
                    message.Add(MessageFactory.GetMessage("negotiation.authentication.ExistingplayerPassword"));
                    message.Add(MessageFactory.GetMessage("system.EchoOff"));
                    _echoOn = false;
                    Require(message, new ValidateDelegate(this.ValidateOldPassword));
                }
                else
                {
                    Finished = true;
                }
            }
        }

        protected override void InitialState()
        {
            Client.Write(new ResourceMessage(MessageType.Information, "negotiation.splash"));
        }

        protected override void FinalState()
        {
            if (_failed)
            {
                Client.Close();
            }
            else
            {
                PlayerFinalizer finalizer = new PlayerFinalizer(Client, GetValue<Player>("player"));
                finalizer.Finalize(GetValue<bool>("isNew"));
            }
            Client.LoginHandler = null;
            Client = null;            
        }

        /// <summary>
        ///     Checks a name to see if it is valid
        /// </summary>
        /// <param name="name">the name to check</param>
        /// <returns>true if valid</returns>
        private bool CheckName(string name) {
            Regex parser = new Regex(@"all|auto|immortal|self|someone|something|the|you|loner|none");
            if (parser.IsMatch(name)) {
                return false;
            }
    
            if (name.Length < 2 || name.Length > 12) {
                return false;
            }

            // check valid characters
            parser = new Regex(@"^[a-zA-Z][a-z0-9]+$");
            if (!parser.IsMatch(name)) {
                return false;
            }

            //TODO: check mob names
	        return true;
        }

        /// <summary>
        /// Validate the player's name.  Check to make sure the name is a valid name,
        /// if it is, then determine if it is an existing player or a new one.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateName(object data)
        {
            string input = (string)data;
            if (input.Length == 0) {
                _failed = true;
                Finished = true;
	            return;
	        }   
	
	        if (!CheckName( input )) {
	            Client.Write(MessageFactory.GetMessage("negotiation.authentication.ErrorIllegalName"));
                return;
	        }

            Player isPlaying = (Player)MudFactory.GetObject<IQueryManager>().Find(new ObjectQuery(null, "Players", new ObjectQuery(input)));
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                Client.Write(MessageFactory.GetMessage("negotiation.authentication.ErrorAlreadyPlaying"));
                return;
            }

            SetValue<string>("name", input);
            Player oldPlayer = (Player) _playerRepository.Load(input);
            if (oldPlayer != null)
            {
                // Old player
                SetValue<bool>("isNew", false);
                SetValue<Player>("player", oldPlayer);
            }
            else
            {
                // New Player
                SetValue<bool>("isNew", true);

                Player player = new Player(input);
                SetValue<Player>("player", player);
            }
        }

        /// <summary>
        /// Confirm a new name for a player and that the player wants to create a new player
        /// and did not mistype the name.
        /// </summary>
        /// <param name="input"></param>
        private void ConfirmName(object data)
        {
            string input = (string)data;
            if (input.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
            {  // Yes
                SetValue<bool>("confirmName", true);
            }
            else if (input.StartsWith("n", StringComparison.CurrentCultureIgnoreCase))
            {  // No
                Client.Write(MessageFactory.GetMessage("negotiation.authentication.EnterAnotherName"));
                Clear();                
            }
            else
            {
                Client.Write(MessageFactory.GetMessage("negotiation.authentication.ErrorConfirmName"));
            }
        }

        /// <summary>
        /// Validate the password of an existing player.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateOldPassword(object data)
        {
            string input = (string)data;
            Client.Write(MessageFactory.GetMessage("system.EchoOn"));
            if (!GetValue<Player>("player").ComparePassword(input))
            {
                Client.Write(MessageFactory.GetMessage("negotiation.authentication.ErrorInvalidPassword"));
                Finished = _failed = true;
                return;
            }
            SetValue<string>("password", input);
        }

        /// <summary>
        /// Accept the initial password for a new player.  Verify that it meets any password
        /// standards.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateNewPassword(object data)
        {
            string input = (string)data;
            Client.Write(MessageFactory.GetMessage("system.EchoOn"));
            if (input.Length < 5)
            {
                Client.Write(MessageFactory.GetMessage("negotiation.authentication.ErrorPasswordLength"));
                return;
            }
            GetValue<Player>("player").SetPassword(input);
            SetValue<string>("password", input);
        }

        /// <summary>
        /// Confirm the players password.  This is the 2nd time the player enters the password and is
        /// validated against their first entry.
        /// </summary>
        /// <param name="input"></param>
        private void ConfirmPassword(object data)
        {
            string input = (string)data;
            Client.Write(MessageFactory.GetMessage("system.EchoOn"));
            _echoOn = true;
            Client.Write(MessageFactory.GetMessage("system.Newline"));
            if (!GetValue<Player>("player").ComparePassword(input))
            {
                Client.Write(MessageFactory.GetMessage("negotiation.authentication.ErrorPasswordsDontMatch"));
                GetValue<Player>("player").SetPassword("");
                Remove("password");
            }
            SetValue<string>("confirmPassword", input);
        }

        private void SetRace(object data)
        {
            string input = (string)data;
            if (string.IsNullOrEmpty(input))
            {
                Client.Write(new StringMessage(MessageType.PlayerError, "InvalidRace", "You must select a race"));
            }
            else
            {
                foreach (Race r in _raceRepository)
                {
                    
                }
            }
        }
    }
}
