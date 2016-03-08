using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Skills;
using Mirage.Core.Messaging;
using Mirage.Core.Command;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.IO.Net;

namespace Mirage.Game.Command
{
    public class PlayerCommands
    {
        private IPlayerRepository _playerRepository;

        public IPlayerRepository PlayerRepository
        {
            get { return this._playerRepository; }
            set { this._playerRepository = value; }
        }

        public ISkillRepository SkillRepository { get; set; }

        [Command(Description = "Saves the current progress")]
        public string save([Actor] IPlayer player)
        {
            PlayerRepository.Save(player);
            return "Information saved.\r\n";
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

        [Command(Description="Lists available skills")]
        public void availSkills([Actor] Player player)
        {
            IPlayerAvailableSkills skills = SkillRepository.GetAvailableSkillsForPlayer(player);
            foreach (AvailableSkill skill in skills.AvailableSkills)
                player.Write(new StringMessage(MessageType.Information, "available skill",
                    string.Format("{0}  {1}\r\n", skill.Skill.Name, skill.Cost)));
        }

        [Command(Description = "Displays the current prompt")]
        public void Prompt([Actor] Player player)
        {
            player.Write(new StringMessage(MessageType.Information, "player.prompt.get", "Prompt set to '" + player.Prompt + "'\r\n"));
        }

        [Command(Description = "Displays the current prompt")]
        public void Prompt([Actor] Player player, [CustomParse] string prompt)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                if (!player.ValidatePrompt(prompt))
                {
                    player.Write(new StringMessage(MessageType.PlayerError, "player.prompt.set.failed", "Prompt is invalid!\r\n"));
                    return;
                }
                player.Prompt = prompt;
                player.Write(new StringMessage(MessageType.Information, "player.prompt.set", "Prompt set to '" + player.Prompt + "'\r\n"));
            }
        }

        [Command(Description = "Displays your score and attributes")]
        public void Score([Actor] Player player)
        {
            var args = new Dictionary<string, object>();
            var sb = new StringBuilder();
            int screenWidth = 0;
            if (player.Client is TextClient) {
                screenWidth = ((TextClient)player.Client).Options.WindowWidth;
            }
            if (screenWidth <= 0) {
                screenWidth = 80;
            }
            screenWidth -= 1; // leave 1 space
            sb.AppendLine(" ".PadRight(screenWidth, '*'));
            sb.AppendLine(" * Name: ${name}".PadRight(screenWidth));
            args["name"] = player.Name;
            sb.AppendLine(" ".PadRight(screenWidth, '*'));
            sb.AppendLine(" *");
            sb.AppendLine(" * Hp: ${hp}/${maxhp}".PadRight(screenWidth));
            args["hp"] = player.HitPoints; args["maxhp"] = player.MaxHitPoints;
            sb.AppendLine(" *");
            sb.AppendLine(" ".PadRight(screenWidth, '*'));
            var msg = MessageFormatter.Instance.Format(player, player, "player.score", sb.ToString(), null, args);
            player.Write(msg);
        }
    }
}
