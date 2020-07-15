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
    public class AddPlayer : ICommand
    {
        public string Command => "CSP_PLAYER_ADD";

        public string[] Aliases => new string[0];

        public string Description => "Adds a player spawn.";

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
            if (args.Length == 3)
            {

                string spawnName = args[1].ToLower().Replace('.', '_');
                if (plugin.db.Spawns.ContainsKey(spawnName))
                {
                    RoleType type = args[2].ToLower().Equals("current") ? plr.RH.characterClassManager.CurClass : (RoleType)Enum.Parse(typeof(RoleType), args[2], true);
                    DatabaseConfigPlayerEntry entry = new DatabaseConfigPlayerEntry()
                    {
                        ClassName = type.ToString(),
                        SpawnName = spawnName
                    };
                    plugin.db.PlayerSpawns.Add(entry);
                    if (plugin.Config.csp_auto_save)
                        plugin.DatabaseSave();
                    response = "Player Spawn added!";
                    return true;
                }
                response = "Spawn not found! Use \"CURRENT\" for the current player role.";
                return true;
            }
            response = "Invalid! Usage: CSP_PLAYER_ADD <SpawnName> <RoleType|CURRENT>";
            return true;
        }
    }
}
