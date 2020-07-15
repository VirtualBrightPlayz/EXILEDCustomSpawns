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
    public class DeleteGroup : ICommand
    {
        public string Command => "CSP_GROUP_DEL";

        public string[] Aliases => new string[0];

        public string Description => "Delete a spawn group.";

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
            var plugin = CustomSpawnPositions.csp;
            var args = arguments.Array;
            args = new string[] { "" }.AddRangeToArray(args);
            if (args.Length == 2)
            {
                if (plugin.db.Groups.ContainsKey(args[1]))
                {
                    plugin.db.Groups.Remove(args[1]);
                    if (plugin.Config.csp_auto_save)
                        plugin.DatabaseSave();
                    response = "Group removed.";
                    return true;
                }
                response = "Group does not exist!";
                return true;
            }
            response = "Invalid! Usage: CSP_GROUP_DEL <GroupName>";
            return true;
        }
    }
}
