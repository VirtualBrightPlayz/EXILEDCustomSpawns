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
    public class ItemAdd : ICommand
    {
        public string Command => "CSP_ITEM_ADD";

        public string[] Aliases => new string[0];

        public string Description => "Add item to spawn.";

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
            if (args.Length == 3 || args.Length == 4 || args.Length == 5)
            {
                string spawnName = args[1].ToLower().Replace('.', '_');
                if (plugin.db.Spawns.ContainsKey(spawnName) && (args.Length < 5 || string.IsNullOrWhiteSpace(args[4]) || plugin.db.Groups.ContainsKey(args[4])))
                {
                    //if (args.Length >= 5 && plugin.db.Groups.ContainsKey(args[4]) || string.IsNullOrWhiteSpace(args[4])
                    ItemType type = (ItemType)Enum.Parse(typeof(ItemType), args[2], true);
                    DatabaseConfigItemEntry entry = new DatabaseConfigItemEntry()
                    {
                        GroupName = args.Length >= 5 ? args[4] : "",
                        ItemName = type.ToString(),
                        SpawnChance = args.Length >= 4 ? float.Parse(args[3]) : 1f,
                        SpawnName = spawnName
                    };
                    plugin.db.ItemSpawns.Add(entry);
                    if (plugin.Config.csp_auto_save)
                        plugin.DatabaseSave();
                    response = "Item Spawn added!";
                    return true;
                }
                response = "Spawn or Group not found!";
                return true;
            }
            response = "Invalid! Usage: CSP_ITEM_ADD <SpawnName> <ItemName> [SpawnChance (0.0-1.0)] [GroupName]";
            return true;
        }
    }
}
