using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DeleteAllItems : ICommand
    {
        public string Command => "CSP_ITEM_DEL_ALL_YES";

        public string[] Aliases => new string[0];

        public string Description => "Clears ALL item spawns.";

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
            plugin.db.ItemSpawns.Clear();
            if (plugin.Config.csp_auto_save)
                plugin.DatabaseSave();
            response = "Item Spawns cleared.";
            return true;
        }
    }
}
