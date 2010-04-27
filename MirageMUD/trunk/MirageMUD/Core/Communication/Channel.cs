using System;
using System.Collections.Generic;
using System.Text;

using Mirage.Core.Data;
using Mirage.Core.Command;
using Mirage.Core.Util;
using JsonExSerializer;
using Mirage.Core.Data.Query;

namespace Mirage.Core.Communication
{
    public class Channel {
        
        private string _name;
        private ISet<string> _allowed;
        private ISet<string> _banned;
        private ISet<IReceiveMessages> _members;
        private ISet<string> _roles;
        private bool _isDefault;
        private IMessageFactory _messageFactory;

        public Channel() : this("", null, null, null)
        {
            
        }

        public Channel(string name, IEnumerable<string> allowed, IEnumerable<string> banned, IEnumerable<string> roles) {
            _name = name;
            _members = new Mirage.Core.Util.HashSet<IReceiveMessages>();
            _allowed = new Mirage.Core.Util.HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            _banned = new Mirage.Core.Util.HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            _roles = new Mirage.Core.Util.HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            if (allowed != null)
                _allowed.Add(allowed);
            if (banned != null)
                _banned.Add(banned);
            if (roles != null)
                _roles.Add(roles);

        }
        
        [ConstructorParameter(0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #region Allow

        /// <summary>
        /// The names of players that are allowed to join, if blank
        /// any player can join as long as they meet security requirements and
        /// they aren't denied.
        /// </summary>
        [ConstructorParameter(1)]
        public IEnumerable<string> Allowed
        {
           get { return _allowed.ToArray(); }
        }

        /// <summary>
        /// Checks to see if the participant is in the allow list
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool IsAllowed(object participant)
        {
            if (_allowed.Count > 0)
            {
                string name = GetName(participant);
                if (name == null)
                    return false;
                return _allowed.Contains(name);
            }
            return true;
        }

        /// <summary>
        /// Allow the player to join.  Once any name is added to this list, anyone who joins
        /// must be in the allow list.  Use ClearAllow list to clear the list
        /// </summary>
        /// <param name="name">the name to add</param>
        public void Allow(string name)
        {
            _allowed.Add(name);
            _banned.Remove(name);
        }

        /// <summary>
        /// Removes the player from the allow list
        /// </summary>
        /// <param name="name">the name to unallow</param>
        public void UnAllow(string name)
        {
            _allowed.Remove(name);
        }

        /// <summary>
        /// Clears all names from the allow list
        /// </summary>
        public void ClearAllow()
        {
            _allowed.Clear();
        }

        #endregion

        #region Ban
        /// <summary>
        /// The players that are not allowed to join
        /// </summary>
        [ConstructorParameter(2)]
        public IEnumerable<string> Banned
        {
            get { return _banned.ToArray(); }
        }

        /// <summary>
        /// Checks to see if the participant is in the banned list
        /// </summary>
        /// <param name="participant">participant to test</param>
        /// <returns>true if denied</returns>
        public bool IsBanned(object participant)
        {
            if (_banned.Count > 0)
            {
                string name = GetName(participant);
                // if no name to test, assume they're denied
                if (name == null)
                    return true;
                return _banned.Contains(name);
            }
            return false;
        }

        /// <summary>
        /// Bans any player with the given name from the channel.  If the
        /// player is already a member of the channel they will be removed and sent a message indicating
        /// that they've been banned.  They will also not be able to join in the future until the
        /// ban is lifted.
        /// </summary>
        /// <param name="name">the name of the player to ban</param>
        public void Ban(string name)
        {
            _banned.Add(name);
            _allowed.Remove(name);
            foreach (IReceiveMessages member in _members)
            {
                if (IsBanned(member))
                {
                    IMessage message = MessageFactory.GetMessage("communication.MemberNotAllowed");
                    message["channel"] = this.Name;
                    member.Write(message);
                    Remove(member);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes the name from the ban list
        /// </summary>
        /// <param name="name"></param>
        public void UnBan(string name)
        {
            _banned.Remove(name);
        }

        /// <summary>
        /// Clears the ban list
        /// </summary>
        public void ClearBans()
        {
            _banned.Clear();
        }
        #endregion

        #region Roles

        /// <summary>
        /// The roles required to join the channel.  A participant must have any one of the
        /// specified roles to join.  If roles are applied to this channel, only objects implementing
        /// IActor(which includes players) may join.
        /// </summary>
        [ConstructorParameter(3)]
        public IEnumerable<string> Roles
        {
            get { return _roles.ToArray(); }
        }

        /// <summary>
        /// Checks to see if the given object is in role for this channel,
        /// tries to find an IPrincipal member for the object
        /// </summary>
        /// <param name="member">the member to check</param>
        /// <returns>true if in role</returns>
        private bool IsInRole(object participant)
        {
            if (_roles.Count > 0)
            {
                IActor actor = participant as IActor;
                if (actor != null)
                {
                    foreach (string role in _roles)
                        if (actor.Principal.IsInRole(role))
                            return true;
                }
                // can't determine their roles, so return false
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the role to this channel.  If no roles exist on the channel, any current members
        /// will be rechecked to see if they can still be members.
        /// </summary>
        /// <param name="role">role to add</param>
        public void AddRole(string role)
        {
            bool recheck = _roles.Count > 0;
            _roles.Add(role);
            if (recheck)
            {
                CheckAllMemberRoles();
            }
        }

        /// <summary>
        /// Removes a role from the restriction list.  Will recheck members to see if they are still allowed
        /// </summary>
        /// <param name="role">the role to remove</param>
        public void RemoveRole(string role)
        {
            _roles.Remove(role);
            CheckAllMemberRoles();
        }

        /// <summary>
        /// Rechecks all members to see if they still meet the role requirements when roles are changed
        /// </summary>
        private void CheckAllMemberRoles()
        {
            // we added a role where there were none before, check to see if
            // members till meet criteria
            // use the Members property since it is readonly we can remove from our list without exceptions
            foreach (IReceiveMessages member in Members)
            {
                if (!IsInRole(member))
                {
                    IMessage message = MessageFactory.GetMessage("communication.MemberNotAllowed");
                    message["channel"] = this.Name;
                    member.Write(message);
                    Remove(member);
                }
            }
        }

        public void ClearRoles()
        {
            _roles.Clear();
        }
        #endregion

        #region Members

        /// <summary>
        /// Gets the current members of the channel
        /// </summary>
        public IEnumerable<IReceiveMessages> Members
        {
            get { return _members.ToArray(); }
        }

        /// <summary>
        /// The number of members in the channel
        /// </summary>
        public int MemberCount
        {
            get { return _members.Count; }
        }

        [JsonExIgnore]
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

        /// <summary>
        /// Checks to see if the specified member is a member of the channel
        /// </summary>
        /// <param name="member">member</param>
        public bool ContainsMember(IReceiveMessages member)
        {
            return _members.Contains(member);
        }

        /// <summary>
        /// Clears all members from the channel
        /// </summary>
        public void ClearMembers()
        {
            // remove manually to clear events
            foreach (IReceiveMessages member in Members)
                Remove(member);
        }

        /// <summary>
        /// Adds the participant to this channel.
        /// </summary>
        /// <param name="participant">the participant to add</param>
        /// <exception cref="Mirage.Core.Communication.ValidationException">The participant is not allowed to join</exception>
        public void Add(IReceiveMessages participant)
        {
            if (CanJoin(participant))
            {
                _members.Add(participant);
                if (participant is IPlayer)
                {
                    ((IPlayer)participant).PlayerEvent += new PlayerEventHandler(Channel_PlayerEvent);
                }
            }
            else
            {
                IMessage message = MessageFactory.GetMessage("communication.CantJoinChannel");
                message["channel"] = this.Name;
                throw new ValidationException(message);
            }
        }

        void Channel_PlayerEvent(object sender, PlayerEventArgs eventArgs)
        {
            if (eventArgs.EventType == PlayerEventType.Quiting)
            {
                Remove((IReceiveMessages) sender);
            }
        }

        /// <summary>
        /// Removes the participant from the channel
        /// </summary>
        /// <param name="participant">participant to remove</param>
        public void Remove(IReceiveMessages participant)
        {
            _members.Remove(participant);
            if (participant is IPlayer)
            {
                ((IPlayer)participant).PlayerEvent -= new PlayerEventHandler(Channel_PlayerEvent);
            }
        }

        #endregion

        /// <summary>
        /// Clears all allow, ban, and role restrictions
        /// </summary>
        public void ClearRestrictions()
        {
            ClearAllow();
            ClearBans();
            ClearRoles();
        }

        /// <summary>
        /// Returns true if the participant can join this channel
        /// </summary>
        /// <param name="participant">participant to check</param>
        /// <returns>true if they can join the channel</returns>
        public bool CanJoin(IReceiveMessages participant)
        {
            return (!IsBanned(participant)
                    && IsAllowed(participant)
                    && IsInRole(participant));
        }

        /// <summary>
        /// Tries to determine the name from any acceptable interface that a name can be retrieved from
        /// </summary>
        /// <param name="participant">participant object to test</param>
        /// <returns>name if it can be determined</returns>
        private string GetName(object participant)
        {
            IUri uri = participant as IUri;
            if (uri != null)
                return uri.Uri;
            return null;
        }

        /// <summary>
        /// Sends the given message to all players within the channel
        /// </summary>
        /// <param name="sender">the message sender</param>
        /// <param name="message">the message to send</param>
        public void Send(IReceiveMessages sender, string messageText) {
            IMessage message = MessageFactory.GetMessage("communication.ChannelText");
            string senderName = GetName(sender) ?? "someone";
            message["sender"] = senderName;
            message["channel"] = this.Name;
            message["message"] = messageText;

            foreach (IReceiveMessages recipient in _members)
                recipient.Write(sender, message);
        }

        /// <summary>
        /// Creates the necessary commands for the player to interact with this channel
        /// </summary>
        public IList<ICommand> CreateCommands()
        {
            List<ICommand> commands = new List<ICommand>();

            commands.Add(new ChannelSendCommand(this, MessageFactory));
            commands.Add(new ChannelToggleCommand(this, MessageFactory));
            return commands;
        }

        /// <summary>
        /// Gets or sets a property that indicates whether this channel should be turned on by 
        /// default for a new player
        /// </summary>
        public bool IsDefault
        {
            get { return this._isDefault; }
            set { this._isDefault = value; }
        }
    }
}
