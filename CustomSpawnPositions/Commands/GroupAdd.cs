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
    public class GroupAdd : ICommand
    {
        public string Command => "CSP_GROUP_ADD";

        public string[] Aliases => new string[0];

        public string Description => "Add a spawn group.";

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
            if (args.Length >= 2 && args.Length <= 5)
            {
                if (!plugin.db.Groups.ContainsKey(args[1]))
                {
                    DatabaseConfigGroupEntry entry = new DatabaseConfigGroupEntry()
                    {
                        SpawnAmount = args.Length >= 4 ? int.Parse(args[3]) : 1,
                        SpawnChance = args.Length >= 3 ? float.Parse(args[2]) : 1.0f,
                        Stackable = args.Length >= 5 ? bool.Parse(args[4]) : false
                    };
                    plugin.db.Groups.Add(args[1], entry);
                    if (plugin.Config.csp_auto_save)
                        plugin.DatabaseSave();
                    response = "Group added.";
                    return true;
                }
                response = "Group already exists!";
                return true;
            }
            response = "Invalid! Usage: CSP_GROUP_ADD <GroupName> [SpawnChance (0.0-1.0)] [SpawnAmount (non-decimal number)] [Stackable (true|false)]";
            return true;
        }
    }
}
