using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;
using JsonExSerializer;
using Mirage.Core.Security;
using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.IO.Net;
using Mirage.Game.World.Query;
using Mirage.Game.World.Skills;
using Mirage.Game.Command;
using Mirage.Core.Messaging;
using Mirage.Core.IO.Net;

namespace Mirage.Game.World
{
    /// <summary>
    ///     A player is controlled by a live person and a participant in
    /// the game (as opposed to a Mobile which is a game object controlled by AI).
    /// It is a descendant of Living, the base-class for living breathing things
    /// </summary>
    public class Player : Living, IPlayer
    {
        private MudPrincipal _principal;
        private string[] _roles;
        public event PlayerEventHandler PlayerEvent;

        /// <summary>
        ///     Creates an instance of a player
        /// </summary>
        public Player() : base()
        {
            this.CommunicationPreferences = new CommunicationPreferences();
            this.Skills = new Dictionary<SkillDefinition, Skill>();
        }

        /// <summary>
        /// Creates a player with the given name and password and security roles
        /// </summary>
        /// <param name="name">player's name</param>
        /// <param name="plainPassword">plaintext password</param>
        /// <param name="roles">the user's security roles</param>
        public Player(string name) : this()
        {
            Uri = name;
            Name = name;
        }

        #region Password Items
        /// <summary>
        ///     The player's password
        /// </summary>
        public string Password { get; set; }       

        /// <summary>
        ///     Sets the password for the character, encrypting it first.
        /// </summary>
        /// <param name="password">plain text password</param>
        public void SetPassword(string password)
        {
            Password = EncryptPassword(password);
        }

        /// <summary>
        ///     Compares another password to the password for this charater
        /// </summary>
        /// <param name="otherPassword">plain text password</param>
        /// <returns>true if the password matches the one for this player</returns>
        public bool ComparePassword(string otherPassword)
        {
            // allow empty password to compare
            if (string.IsNullOrEmpty(otherPassword) &&
                string.IsNullOrEmpty(Password))
            {
                return true;
            }
            else
            {
                return EncryptPassword(otherPassword).Equals(Password);
            }
        }

        /// <summary>
        ///     Encrypts a password for storage or comparison.  The
        /// encryption is one way.
        /// </summary>
        /// <param name="password">the password to encrypt</param>
        /// <returns>the encrypted password</returns>
        private string EncryptPassword(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes(Uri.ToLower().PadRight(8,'x'));
            Rfc2898DeriveBytes passwordKey = new Rfc2898DeriveBytes("MirageHashPassword", salt);
            byte[] secretKey = passwordKey.GetBytes(64);
            HMACSHA1 hash = new HMACSHA1(secretKey);

            byte[] bytesIn = Encoding.ASCII.GetBytes(password);
            byte[] bytesOut = hash.ComputeHash(bytesIn);
            string encrypted = Convert.ToBase64String(bytesOut);
            return encrypted;
        }

        #endregion Password Items

        /// <summary>
        ///    Gets or sets the client connection for this player
        /// </summary>
        [XmlIgnore]
        [JsonExIgnore]
        public IClient<ClientPlayerState> Client { get; set; }

        /// <summary>
        ///     The Command interpreters in effect for this player
        /// </summary>
        [XmlIgnore]
        [JsonExIgnore]
        public IInterpret Interpreter { get; set; }

        /// <summary>
        /// The uri to the room that they are in
        /// </summary>
        public string RoomUri
        {
            get
            {
                Room room = Room;
                if (room != null)
                {
                    return room.FullUri;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    Room = (Room)MudFactory.GetObject<MudWorld>().ResolveUri(value);
                    if (Room == null)
                    {
                        throw new ObjectNotFoundException("Could not find room with value: " + value);
                    }
                }
                else
                {
                    Room = null;
                }
            }
        }

        [XmlIgnore]
        [JsonExIgnore]
        public override string FullUri
        {
            get
            {
                return "Players/" + this.Uri;
            }
        }

        public override void Write(IMessage message)
        {
            Write(null, message);
        }

        public override void Write(object sender, IMessage message)
        {
            if (sender is IPlayer && this.CommunicationPreferences.IsIgnored(((IPlayer)sender).Name))
                return;

            if (Client != null)
                Client.Write(message);
        }

        /// <summary>
        /// Security principal for the player
        /// </summary>
        public override IPrincipal Principal
        {
            get {
                if (_principal == null)
                {
                    if (_roles == null)
                        _principal = new MudPrincipal(new MudIdentity(Uri));
                    else
                        _principal = new MudPrincipal(new MudIdentity(Uri), _roles);
                    _roles = null;
                }
                return _principal; 
            }
        }

        /// <summary>
        /// Gets or sets the roles that the user has. Normally one should not call set, but
        /// use the principal object to add the roles.
        /// </summary>
        public string[] Roles
        {
            get
            {
                if (_principal != null)
                    return _principal.Roles;
                else
                    return _roles;
            }
            set
            {
                _roles = value;
                if (_principal != null)
                {
                    _principal.Clear();
                    _principal.AddRoles(value);
                }
            }
        }

        public void FirePlayerEvent(PlayerEventType eventType)
        {
            PlayerEvent(this, new PlayerEventArgs(eventType));
        }

        public override string ToString()
        {
            return this.GetType().Name + " " + Name;
        }

        public CommunicationPreferences CommunicationPreferences { get; set; }

        [JsonExProperty]
        public IDictionary<SkillDefinition, Skill> Skills { get; private set; }
    }
}
