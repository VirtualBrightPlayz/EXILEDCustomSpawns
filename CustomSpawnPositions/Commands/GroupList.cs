using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Harmony;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GroupList : ICommand
    {
        public string Command => "CSP_GROUP_LIST";

        public string[] Aliases => new string[0];

        public string Description => "List all spawn groups.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "";
                return false;
            }
            var plr = sender as PlayerCommandSender;
            var player = new Player(plr.RH);
            if (!player.CheckPermission("customspawns.manage"))
            {
                response = "";
                return false;
            }
            List<string> items = new List<string>();
            foreach (var item in CustomSpawnPositions.csp.db.Groups)
            {
                items.Add(item.Key + ": " + item.Value.ToString());
            }
            response = "All Spawn Groups: " + items.Join();
            return true;
        }
    }
}
