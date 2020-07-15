using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Harmony;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddSpawn : ICommand
    {
        public string Command => "CSP_SPAWN_ADD";

        public string[] Aliases => new string[0];

        public string Description => "Add custom spawn";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                var player = new Player(plr.RH);
                if (!player.CheckPermission("customspawns.manage"))
                {
                    response = "";
                    return false;
                }
                var plugin = CustomSpawnPositions.csp;
                var args = arguments.Array;
                args = new string[] { "" }.AddRangeToArray(args); // im lazy af. lmao.
                {
                    if (args.Length == 3 || (args.Length == 4 && !bool.Parse(args[3])))
                    {
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            response = "Spawn already exists!";
                            return true;
                        }
                        if (args[2].ToLower().Equals("none"))
                        {
                            Vector3 pos2 = player.Position;
                            DatabaseConfigSpawnEntry entry = new DatabaseConfigSpawnEntry()
                            {
                                RoomName = "none",
                                Position = new SpawnPosition()
                                {
                                    X = pos2.x,
                                    Y = pos2.y,
                                    Z = pos2.z
                                }
                            };
                            plugin.db.Spawns.Add(spawnName, entry);
                            ItemType.Coin.Spawn(0f, pos2);
                            if (plugin.Config.csp_auto_save)
                                plugin.DatabaseSave();
                            response = "Spawn added.";
                            return true;
                        }
                        Room final = null;
                        float dist = float.MaxValue;
                        foreach (var room in Map.Rooms)
                        {
                            if ((room.Name.ToLower().StartsWith(args[2].ToLower()) || args[2].ToLower().Equals("close")) && Vector3.Distance(room.Transform.position, player.Position) < dist)
                            {
                                dist = Vector3.Distance(room.Transform.position, player.Position);
                                final = room;
                            }
                        }
                        if (final != null)
                        {
                            Vector3 pos2 = final.Transform.InverseTransformPoint(player.Position);
                            DatabaseConfigSpawnEntry entry = new DatabaseConfigSpawnEntry()
                            {
                                RoomName = /*args[2].ToLower().Equals("close") ?*/ final.Name.Split(' ')[0] /*: args[2].ToLower()*/,
                                Position = new SpawnPosition()
                                {
                                    X = pos2.x,
                                    Y = pos2.y,
                                    Z = pos2.z
                                }
                            };
                            plugin.db.Spawns.Add(spawnName, entry);
                            ItemType.Coin.Spawn(0f, pos2);
                            if (plugin.Config.csp_auto_save)
                                plugin.DatabaseSave();
                            response = "Spawn added.";
                            return true;
                        }
                        response = "Room not found! Use \"CLOSE\" if you want to use the closest room. Use \"NONE\" if it is room independent (ex. surface zone).";
                        return true;
                    }
                    else if (args.Length == 4 && bool.Parse(args[3]))
                    {
                        //player.plyMovementSync.Rotations;
                        Vector3 dir = plr.RH.PlayerCameraReference.forward; //new Vector3(player.plyMovementSync.Rotations.x, player.plyMovementSync.Rotations.y, 0f);
                        RaycastHit output;
                        if (!Physics.Raycast(plr.RH.PlayerCameraReference.position, dir, out output, 100f))
                        {
                            response = "Look at something, not nothing!";
                            return true;
                        }
                        Vector3 pos = output.point;
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            response = "Spawn already exists!";
                            return true;
                        }
                        if (args[2].ToLower().Equals("none"))
                        {
                            Vector3 pos2 = pos;
                            DatabaseConfigSpawnEntry entry = new DatabaseConfigSpawnEntry()
                            {
                                RoomName = "none",
                                Position = new SpawnPosition()
                                {
                                    X = pos2.x,
                                    Y = pos2.y,
                                    Z = pos2.z
                                }
                            };
                            plugin.db.Spawns.Add(spawnName, entry);
                            if (plugin.Config.csp_auto_save)
                                plugin.DatabaseSave();
                            response = "Spawn added.";
                            return true;
                        }
                        Room final = null;
                        float dist = float.MaxValue;
                        foreach (var room in Map.Rooms)
                        {
                            if ((room.Name.ToLower().StartsWith(args[2].ToLower()) || args[2].ToLower().Equals("close")) && Vector3.Distance(room.Transform.position, pos) < dist)
                            {

                                dist = Vector3.Distance(room.Transform.position, pos);
                                final = room;
                            }
                        }
                        if (final != null)
                        {
                            Vector3 pos2 = final.Transform.InverseTransformPoint(pos);
                            DatabaseConfigSpawnEntry entry = new DatabaseConfigSpawnEntry()
                            {
                                RoomName = /*args[2].ToLower().Equals("close") ?*/ final.Name.Split(' ')[0] /*: args[2].ToLower()*/,
                                Position = new SpawnPosition()
                                {
                                    X = pos2.x,
                                    Y = pos2.y,
                                    Z = pos2.z
                                }
                            };
                            plugin.db.Spawns.Add(spawnName, entry);
                            ItemType.Coin.Spawn(0f, pos);
                            if (plugin.Config.csp_auto_save)
                                plugin.DatabaseSave();
                            response = "Spawn added.";
                            return true;
                        }
                        response = "Room not found! Use \"CLOSE\" if you want to use the closest room. Use \"NONE\" if it is room independent (ex. surface zone).";
                        return true;
                    }
                    response = "Invalid! Usage: CSP_SPAWN_ADD <SpawnName> <RoomName|CLOSE|NONE> [Looking(true|false)]";
                    return true;
                }
            }
            response = "";
            return false;
        }
    }
}
