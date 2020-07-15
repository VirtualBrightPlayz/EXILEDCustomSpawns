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
    public class DeleteSpawn : ICommand
    {
        public string Command => "CSP_SPAWN_DEL";

        public string[] Aliases => new string[0];

        public string Description => "Delete a spawn";

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
                string spawnName = args[1].ToLower().Replace('.', '_');
                if (plugin.db.Spawns.ContainsKey(spawnName))
                {
                    plugin.db.Spawns.Remove(spawnName);
                    if (plugin.Config.csp_auto_save)
                        plugin.DatabaseSave();
                    response = "Spawn removed.";
                    return true;
                }
            }
            response = "Invalid! Usage: CSP_SPAWN_DEL <SpawnName>";
            return true;
        }
    }
}
