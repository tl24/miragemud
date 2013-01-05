using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;
using Mirage.IO.Net;
using Mirage.Core;
using System.IO;
using System.Configuration;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// StateHandler class that controls Login and player creation for the text client.
    /// </summary>
    public class TextLoginStateHandler : AbstractStateMachine
    {
        private bool _failed;
        private IPlayerRepository _playerRepository;
        private IRaceRepository _raceRepository;

        public TextLoginStateHandler(IConnectionAdapter client)
            : base(client)
        {
            _failed = false;
            _playerRepository = MudFactory.GetObject<IPlayerRepository>();
            _raceRepository = MudFactory.GetObject<IRaceRepository>();
        }
       
        private IMessage FormatMessage(MessageDefinition definition, object args = null)
        {
            if (args != null)
            {
                var tmp = ReflectionUtils.ObjectToDictionary(args);
                tmp[MessageFormatter.AddNewLine] = false;
                args = tmp;
            }
            else
            {
                args = new Dictionary<string, object> { { MessageFormatter.AddNewLine, false } };
            }
            var msg = MessageFormatter.Instance.Format(null, null, definition, null, args);
            if (msg.MessageType != MessageType.PlayerError)
                msg.MessageType = MessageType.Prompt;
            return msg;
        }

        protected override void DetermineNextState()
        {
            if (!Contains("name"))
                Require(FormatMessage(LoginAndPlayerCreationMessages.EnterName), new ValidateDelegate(this.ValidateName));
            else if (GetValue<bool>("isNew") == true) {
                if (!Contains("confirmName")) {
                    IMessage rm = FormatMessage(LoginAndPlayerCreationMessages.ConfirmNewName, new {player = GetValue<Player>("player").Title});
                    Require(rm, new ValidateDelegate(this.ConfirmName));
                }
                else if (!Contains("password"))
                {
                    IMessage prompt = FormatMessage(LoginAndPlayerCreationMessages.NewplayerPassword, new { player = GetValue<Player>("player").Title });
                    IMessage echoOff = FormatMessage(CommonMessages.EchoOff); 

                    Require(new [] { prompt, echoOff }, new ValidateDelegate(this.ValidateNewPassword));
                }
                else if (!Contains("confirmPassword"))
                {
                    IMessage prompt = FormatMessage(LoginAndPlayerCreationMessages.ConfirmPassword);
                    IMessage echoOff = FormatMessage(CommonMessages.EchoOff);

                    Require(new[] { prompt, echoOff }, new ValidateDelegate(this.ConfirmPassword));
                }
                else
                {
                    Finished = true;
                }
            } else {
                if (!Contains("password"))
                {
                    List<IMessage> message = new List<IMessage>();
                    message.Add(FormatMessage(LoginAndPlayerCreationMessages.ExistingplayerPassword));
                    message.Add(FormatMessage(CommonMessages.EchoOff));
                    Require(message, new ValidateDelegate(this.ValidateOldPassword));
                }
                else
                {
                    Finished = true;
                }
            }
        }

        private string _splashScreenText;

        public string SplashScreenText
        {
            get
            {
                if (_splashScreenText == null)
                {
                    _splashScreenText = File.ReadAllText(ConfigurationManager.AppSettings["textclient.splash"]);
                }
                return _splashScreenText;
            }
            set
            {
                _splashScreenText = value;
            }
        }
        protected override void InitialState()
        {
            Client.Write(new StringMessage(MessageType.Information, "negotiation.splash", SplashScreenText));
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
	            Client.Write(FormatMessage(LoginAndPlayerCreationMessages.ErrorIllegalName));
                return;
	        }

            Player isPlaying = (Player)MudFactory.GetObject<MudWorld>().Players.FindOne(input, QueryMatchType.Exact);
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                Client.Write(FormatMessage(LoginAndPlayerCreationMessages.ErrorAlreadyPlaying));
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
                Client.Write(FormatMessage(LoginAndPlayerCreationMessages.EnterAnotherName));
                Clear();                
            }
            else
            {
                Client.Write(FormatMessage(LoginAndPlayerCreationMessages.ErrorConfirmName));
            }
        }

        /// <summary>
        /// Validate the password of an existing player.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateOldPassword(object data)
        {
            string input = (string)data;
            Client.Write(FormatMessage(CommonMessages.EchoOn));
            if (!GetValue<Player>("player").ComparePassword(input))
            {
                Client.Write(FormatMessage(LoginAndPlayerCreationMessages.ErrorWrongPassword));
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
            Client.Write(FormatMessage(CommonMessages.EchoOn));
            if (input.Length < 5)
            {
                Client.Write(FormatMessage(LoginAndPlayerCreationMessages.ErrorPasswordLength));
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
            Client.Write(FormatMessage(CommonMessages.EchoOn));
            Client.Write(FormatMessage(CommonMessages.Newline));
            if (!GetValue<Player>("player").ComparePassword(input))
            {
                Client.Write(FormatMessage(LoginAndPlayerCreationMessages.ErrorPasswordsDontMatch));
                GetValue<Player>("player").SetPassword("");
                Remove("password");
                Remove("confirmPassword");
            }
            else
            {
                SetValue<string>("confirmPassword", input);
            }
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
