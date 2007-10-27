using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Mirage.Core.IO;
using Mirage.Core.Command;

using Mirage.Core.IO.Serialization;
using Mirage.Core.Data.Query;
using Mirage.Core.Communication;
using System.Security.Principal;
using JsonExSerializer;
using Mirage.Core.Data;
using Mirage.Core.Security;
using Mirage.Core;

namespace Mirage.Stock.Data
{
    /// <summary>
    ///     A player is controlled by a live person and a participant in
    /// the game (as opposed to a Mobile which is a game object controlled by AI).
    /// It is a descendant of Living, the base-class for living breathing things
    /// </summary>
    public class Player : Living, IPlayer
    {
        private string _password;
        private IClient _client;
        private IInterpret _interpreter;
        private MudPrincipal _principal;
        private string[] _roles;
        private CommunicationPreferences _commPrefs = new CommunicationPreferences();

        public event PlayerEventHandler PlayerEvent;

        /// <summary>
        ///     Creates an instance of a player
        /// </summary>
        public Player() : base()
        {
        }

        /// <summary>
        /// Creates a player with the given name and password and security roles
        /// </summary>
        /// <param name="name">player's name</param>
        /// <param name="plainPassword">plaintext password</param>
        /// <param name="roles">the user's security roles</param>
        public Player(string name)
        {
            Uri = name;
            Title = name;
        }

        #region Password Items
        /// <summary>
        ///     The player's password
        /// </summary>
        public string Password        
        {            
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        ///     Sets the password for the character, encrypting it first.
        /// </summary>
        /// <param name="password">plain text password</param>
        public void SetPassword(string password)
        {
            _password = EncryptPassword(password);
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
                string.IsNullOrEmpty(_password))
            {
                return true;
            }
            else
            {
                return EncryptPassword(otherPassword).Equals(_password);
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
        public IClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        /// <summary>
        ///     The Command interpreters in effect for this player
        /// </summary>
        [XmlIgnore]
        [JsonExIgnore]
        public IInterpret Interpreter
        {
            get { return _interpreter; }
            set { _interpreter = value; }
        }

        /// <summary>
        /// The uri to the room that they are in
        /// </summary>
        public string RoomUri
        {
            get
            {
                Room room = Container as Room;
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
                    Container = (Room)MudFactory.GetObject<IQueryManager>().Find(value);
                    if (Container == null)
                    {
                        throw new ObjectNotFoundException("Could not find room with value: " + value);
                    }
                }
                else
                {
                    Container = null;
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
        /// <summary>
        ///     Loads a player object from a file
        /// </summary>
        /// <param name="name">the name of the player to load</param>
        /// <returns></returns>
        public static Player Load(string uri)
        {
            IPersistenceManager persister = ObjectStorageFactory.GetPersistenceManager(typeof(Player));
            try
            {
                Player p = (Player) persister.Load(uri);
                return p;
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }

        /// <summary>
        ///     Saves the given player to disk
        /// </summary>
        /// <param name="p">the player to save</param>
        public static void Save(Player p)
        {
            IPersistenceManager persister = ObjectStorageFactory.GetPersistenceManager(p.GetType());
            persister.Save(p, p.Uri);
        }

        #region Commands

        [Command(Description = "Attempt to kill another player or mobile")]
        public string kill([Actor] Player self, string target)
        {
            return "You are going to kill " + target + "\r\n";
        }

        [Command(Description = "Change the password")]
        public string changePassword(string oldPassword, string newPassword)
        {
            if (ComparePassword(oldPassword))
            {
                SetPassword(newPassword);
                return "Password changed.\r\n";
            }
            else
            {
                return "The old password is incorrect.\r\n";
            }
        }

        [Command(Description = "Attempt to kill another player or mobile")]
        public string kill([Actor] Player self, string target, int count)
        {
            return "You are going to kill " + target + " " + count + " times\r\n";
        }

        [Command(Description = "Attempt to kill another player or mobile")]
        public string kill([Actor] Player self, 
                          [Lookup("/Players")] Player target)
        {
            return "You started a fight with " + target.Title + ".\r\n";
        }

        [Command(Description="Saves the current progress")]
        [Confirmation(CancellationMessage="Save cancelled.\r\n")]
        public string save()
        {
            Player.Save(this);
            return "Information saved.\r\n";
        }

        public void FirePlayerEvent(PlayerEventType eventType)
        {
            PlayerEvent(this, new PlayerEventArgs(eventType));
        }
        #endregion

        public override string ToString()
        {
            return this.GetType().Name + " " + Title;
        }

        public ICommunicationPreferences CommunicationPreferences
        {
            get { return this._commPrefs; }
            set { this._commPrefs = (CommunicationPreferences) value; }
        }

    }
}
