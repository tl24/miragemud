using System;
using System.Collections.Generic;
using System.Text;

using Mirage.Core.Data;
using Mirage.Core.Command;

namespace Mirage.Core.Communication
{
    public class Channel {
        /*
        private string _name;
        private IList<IPrincipal> _allowed;
        private IList<IPrincipal> _denied;
        private IDictionary<string, Player> _members;
        
        public Channel(string name) {
           this._name = _name;
        }

        //TODO: Visibility?? Readonly?
        public IList<IPrincipal> Allowed {
           get { return _allowed; }
           set { _allowed = value; }
        }

        public IList<IPrincipal> Denied {
           get { return _denied; }
           set { _denied = value; }
        }

        public bool CanJoin() {
           if (_denied.Count > 0) {
              foreach(? role in _denied) {
                 if IsPlayerInRole(player, role)) {
                     return false;
                 }
              }
           }

           if (_allowed.Count > 0) {
              foreach(? role in _allowed) {
                 if IsPlayerInRole(player, role)) {
                     return true;
                 }
              }
              return false;
           } else {
              return true;
           }
        }

        public void Join(Player player) {
            if (CanJoin(player)) {
                _members[player.Uri] = player;
            } else {
                throw new ApplicationException("Player " + player.Uri + " can't join channel " + _name);
            }
        }
                 
        public void Part(Player player) { 
            if (_members.Contains(player)) {
                _members.Remove(player);
            }
        }

        private bool IsPlayerInRole(Player player, string role) {
            //TODO: Check role for player
        }

        public int Send(Player sender, string message) {
            string formatted = String.format("[{0}]: {1}\r\n", _name, message);
            foreach(Player recipient in _members) {
               //TODO: Check preferences
               if (recipient != sender) {
                   recipient.writeToBuffer(formatted);
               }
            }
        }
        */

        [Command(Aliases=new string[]{"test"})]
        public static void Send(string name, [Actor] IActor player) {
            
        }        

        [Command]
        public static void Send(string name, [Actor] IActor player, [CustomParse] string message)
        {
           
        }        
    }
}
