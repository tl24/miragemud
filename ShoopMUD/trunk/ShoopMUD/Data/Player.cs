using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Shoop.IO;
using Shoop.Command;
using Shoop.Attributes;
using Shoop.IO.Serialization;
using Newtonsoft.Json;

namespace Shoop.Data
{
    /// <summary>
    ///     A player is controlled by a live person and a participant in
    /// the game (as opposed to a Mobile which is a game object controlled by AI).
    /// It is a descendant of Animate, the base-class for living breating things
    /// </summary>
    public class Player : Animate
    {
        private static string _playerDir;
        private static bool _initted;

        private string _description;
        private string _password;
        private Descriptor _descriptor;
        private IInterpret _interpreter;

        /// <summary>
        ///     Creates an instance of a player
        /// </summary>
        public Player() : base()
        {            
        }

        /// <summary>
        ///     The player's password
        /// </summary>
        public string password        
        {            
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        ///     Sets the password for the character, encrypting it first.
        /// </summary>
        /// <param name="password">plain text password</param>
        public void setPassword(string password)
        {
            _password = encryptPassword(password);
        }

        /// <summary>
        ///     Compares another password to the password for this charater
        /// </summary>
        /// <param name="otherPassword">plain text password</param>
        /// <returns>true if the password matches the one for this player</returns>
        public bool comparePassword(string otherPassword)
        {
            // allow empty password to compare
            if ((otherPassword == null || otherPassword == string.Empty) &&
                (_password == null || _password == string.Empty))
            {
                return true;
            }
            else
            {
                return encryptPassword(otherPassword).Equals(_password);
            }
        }

        /// <summary>
        ///     Encrypts a password for storage or comparison.  The
        /// encryption is one way.
        /// </summary>
        /// <param name="password">the password to encrypt</param>
        /// <returns>the encrypted password</returns>
        private string encryptPassword(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes("encryptPassword");
            Rfc2898DeriveBytes passwordKey = new Rfc2898DeriveBytes("ROMHashPassword", salt);
            byte[] secretKey = passwordKey.GetBytes(64);
            HMACSHA1 hash = new HMACSHA1(secretKey);

            byte[] bytesIn = Encoding.ASCII.GetBytes(password);
            byte[] bytesOut = hash.ComputeHash(bytesIn);
            string encrypted = Convert.ToBase64String(bytesOut);
            return encrypted;
        }
        /// <summary>
        ///     The player's Description
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        ///    Gets or sets the descriptor for this player
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public Descriptor Descriptor
        {
            get { return _descriptor; }
            set { _descriptor = value; }
        }

        /// <summary>
        ///     The Command interpreters in effect for this player
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IInterpret interpreter
        {
            get { return _interpreter; }
            set { _interpreter = value; }
        }

        /// <summary>
        ///     Retrieve configuration settings
        /// </summary>
        private static void initConfigSettings()
        {
            if (!_initted)
            {
                NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
                _playerDir = allAppSettings.Get("player.dir");
                _initted = true;
            }
        }

        /// <summary>
        ///     Loads a player object from a file
        /// </summary>
        /// <param name="name">the name of the player to load</param>
        /// <returns></returns>
        public static Player load(string uri)
        {
            initConfigSettings();
            IObjectDeserializer deserializer = ObjectSerializerFactory.getDeserializer(_playerDir, typeof(Player), uri.ToLower());
            try
            {
                Player p = (Player) deserializer.Deserialize(uri);
                return p;
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }

        [Command(Description = "Attempt to kill another player or mobile")]
        public string kill([ArgumentType(ArgumentType.Self)] Player self, string target)
        {
            return "You are going to kill " + target + "\r\n";
        }

        [Command(Description = "Change the password")]
        public string changePassword(string oldPassword, string newPassword)
        {
            if (comparePassword(oldPassword))
            {
                setPassword(newPassword);
                return "Password changed.\r\n";
            }
            else
            {
                return "The old password is incorrect.\r\n";
            }
        }

        [Command(Description = "Attempt to kill another player or mobile")]
        public string kill([ArgumentType(ArgumentType.Self)] Player self, string target, int count)
        {
            return "You are going to kill " + target + " " + count + " times\r\n";
        }

        [Command(Description="Attempt to kill another player or mobile")]
        public string kill([ArgumentType(ArgumentType.Self)] Player self, 
                           [ArgumentType(ArgumentType.Player, Scope=ScopeType.Room)] Player target)
        {
            return "You started a fight.\r\n";
        }

        [Command(Description="Saves the current progress")]
        [Confirmation(CancellationMessage="Save cancelled.\r\n")]
        public string save()
        {
            Player.save(this);
            return "Information saved.\r\n";
        }

        /*
        [Command(Description="Lists available commands")]
        public string commands()
        {
            
            StringBuilder sb = new StringBuilder();
            Dictionary<string, rom.Command.MethodHelper> typeCache = rom.Command.MethodHelper.getTypeCache(typeof(Player));
            sb.Append("Available Commands for Player:\r\n");
            foreach (MethodHelper entry in typeCache.Values)
            {
                sb.Append(entry.Name);
                sb.Append('\t');
                sb.Append(entry.Description);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        */

        /// <summary>
        ///     Saves the given player to disk
        /// </summary>
        /// <param name="p">the player to save</param>
        public static void save(Player p)
        {
            initConfigSettings();
            IObjectSerializer serializer = ObjectSerializerFactory.getSerializer(_playerDir, p.GetType());
            serializer.Serialize(p, p.URI);
        }
    }
}
