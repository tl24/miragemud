using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Command;
using Mirage.Stock.Data;

namespace Mirage.Stock.Command
{
    public class PlayerCommands
    {
        private IPlayerRepository _playerRepository;

        public IPlayerRepository PlayerRepository
        {
            get { return this._playerRepository; }
            set { this._playerRepository = value; }
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

        [Command(Description = "Saves the current progress")]
        public string save([Actor] IPlayer player)
        {
            PlayerRepository.Save(player);
            return "Information saved.\r\n";
        }

        [Command(Description = "Attempt to kill another player or mobile")]
        public string kill([Actor] Player self, string target)
        {
            return "You are going to kill " + target + "\r\n";
        }

        [Command(Description = "Change the password")]
        public string changePassword([Actor] Player player, string oldPassword, string newPassword)
        {
            if (player.ComparePassword(oldPassword))
            {
                player.SetPassword(newPassword);
                return "Password changed.\r\n";
            }
            else
            {
                return "The old password is incorrect.\r\n";
            }
        }
    }
}
