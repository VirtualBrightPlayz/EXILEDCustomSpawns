using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EXILED;
using EXILED.ApiObjects;
using EXILED.Extensions;
using Harmony;
using UnityEngine;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions
{
    public class CSPEventHandler
    {
        private CustomSpawnPositions plugin;

        public CSPEventHandler(CustomSpawnPositions customSpawnPositions)
        {
            this.plugin = customSpawnPositions;
        }

        internal void RACmd(ref RACommandEvent ev)
        {
            string[] args = ev.Command.Split(' ');
            //Player.StrHubs[ev.Sender.SenderId].CheckPermission("roundmod.setmod");
            ReferenceHub player = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? PlayerManager.localPlayer.GetPlayer() : Player.GetPlayer(ev.Sender.SenderId);
            if (player.CheckPermission("customspawns.reload"))
            {
                if (args[0].ToUpper().Equals("CSP_RELOAD"))
                {
                    plugin.ConfigLoad();
                    plugin.DatabaseLoad();
                    ev.Allow = true;
                    ev.Sender.RAMessage("CustomSpawns config reloaded!", pluginName: plugin.getName);
                }
            }
            if (player.CheckPermission("customspawns.manage") && player != PlayerManager.localPlayer.GetPlayer())
            {
                if (args[0].ToUpper().Equals("CSP_SPAWN_ADD"))
                {
                    if (args.Length == 3)
                    {
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            ev.Allow = true;
                            ev.Sender.RAMessage("Spawn already exists!", pluginName: plugin.getName);
                            return;
                        }
                        Room final = null;
                        float dist = float.MaxValue;
                        foreach (var room in Map.Rooms)
                        {
                            if (room.Name.ToLower().StartsWith(args[2].ToLower()) && Vector3.Distance(room.Transform.position, player.GetPosition()) < dist)
                            {
                                dist = Vector3.Distance(room.Transform.position, player.GetPosition());
                                final = room;
                            }
                        }
                        if (final != null)
                        {
                            Vector3 pos2 = final.Transform.InverseTransformPoint(player.GetPosition());
                            DatabaseConfigSpawnEntry entry = new DatabaseConfigSpawnEntry()
                            {
                                RoomName = args[2].ToLower(),
                                Position = new SpawnPosition()
                                {
                                    X = pos2.x,
                                    Y = pos2.y,
                                    Z = pos2.z
                                }
                            };
                            plugin.db.Spawns.Add(spawnName, entry);
                            ev.Allow = true;
                            ev.Sender.RAMessage("Spawn added.", pluginName: plugin.getName);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            return;
                        }
                        ev.Allow = true;
                        ev.Sender.RAMessage("Room not found!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = true;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_SPAWN_ADD <SpawnName> <RoomName>", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_SPAWN_LIST"))
                {
                    ev.Allow = true;
                    List<string> items = new List<string>();
                    foreach (var item in plugin.db.Spawns)
                    {
                        items.Add(item.Key + ": " + item.Value.ToString());
                    }
                    ev.Sender.RAMessage("All Spawn Points: " + items.Join(), pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_SPAWN_DEL"))
                {
                    if (args.Length == 2)
                    {
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            plugin.db.Spawns.Remove(spawnName);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = true;
                            ev.Sender.RAMessage("Spawn removed.", pluginName: plugin.getName);
                            return;
                        }
                    }
                    ev.Allow = true;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_SPAWN_DEL <SpawnName>", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_LIST_ITEMS"))
                {
                    ev.Allow = true;
                    List<string> items = new List<string>();
                    foreach (var item in Enum.GetValues(typeof(ItemType)))
                    {
                        items.Add(item.ToString());
                    }
                    ev.Sender.RAMessage("All Valid Items: " + items.Join(), pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_LIST_ROOMS"))
                {
                    ev.Allow = true;
                    List<string> rooms = new List<string>();
                    foreach (var item in Map.Rooms)
                    {
                        rooms.Add(item.Name);
                    }
                    ev.Sender.RAMessage("All Rooms in current map: " + rooms.Join(), pluginName: plugin.getName);
                    ev.Sender.RAMessage("Use the room name! Not the number! (use \"ClassDSpawn\" not \"ClassDSpawn (22)\")", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_GROUP_ADD"))
                {
                    if (args.Length >= 2 && args.Length <= 5)
                    {
                        if (!plugin.db.Groups.ContainsKey(args[1]))
                        {
                            DatabaseConfigGroupEntry entry = new DatabaseConfigGroupEntry()
                            {
                                SpawnAmount = args.Length >= 4 ? int.Parse(args[3]) : 1,
                                SpawnChance = args.Length >= 3 ? float.Parse(args[2]) : 1.0f,
                                Stackable = args.Length >= 5 ? bool.Parse(args[4]) : true
                            };
                            plugin.db.Groups.Add(args[1], entry);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = true;
                            ev.Sender.RAMessage("Group added.", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = true;
                        ev.Sender.RAMessage("Group already exists!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = true;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_GROUP_ADD <GroupName> [SpawnChance (0.0-1.0)] [SpawnAmount (non-decimal number)] [Stackable (true|false)]", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_GROUP_LIST"))
                {
                    ev.Allow = true;
                    List<string> items = new List<string>();
                    foreach (var item in plugin.db.Groups)
                    {
                        items.Add(item.Key + ": " + item.Value.ToString());
                    }
                    ev.Sender.RAMessage("All Spawn Groups: " + items.Join(), pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_GROUP_DEL"))
                {
                    if (args.Length == 2)
                    {
                        if (plugin.db.Groups.ContainsKey(args[1]))
                        {
                            plugin.db.Groups.Remove(args[1]);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = true;
                            ev.Sender.RAMessage("Group removed.", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = true;
                        ev.Sender.RAMessage("Group does not exist!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = true;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_GROUP_DEL <GroupName>", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_ITEM_ADD"))
                {
                    if (args.Length == 3 || args.Length == 4 || args.Length == 5)
                    {
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName) && (plugin.db.Groups.ContainsKey(args[4]) || string.IsNullOrWhiteSpace(args[4])))
                        {
                            ItemType type = (ItemType)Enum.Parse(typeof(ItemType), args[2], true);
                            DatabaseConfigItemEntry entry = new DatabaseConfigItemEntry()
                            {
                                GroupName = args.Length >= 5 ? args[4] : "",
                                ItemName = type.ToString(),
                                SpawnChance = args.Length >= 4 ? float.Parse(args[3]) : 1f,
                                SpawnName = spawnName
                            };
                            plugin.db.ItemSpawns.Add(entry);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = true;
                            ev.Sender.RAMessage("Item Spawn added!", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = true;
                        ev.Sender.RAMessage("Spawn or Group not found!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = true;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_ITEM_ADD <SpawnName> <ItemName> [SpawnChance (0.0-1.0)] [GroupName]", pluginName: plugin.getName);
                    return;
                }
            }
        }

        internal void RoundStart()
        {
            /*foreach (Room room in Map.Rooms)
            {
                Log.Info(room.Name);
            }*/
            Dictionary<string, List<DatabaseConfigItemEntry>> groups = new Dictionary<string, List<DatabaseConfigItemEntry>>();
            foreach (var group in plugin.db.Groups)
            {
                groups.Add(group.Key, new List<DatabaseConfigItemEntry>());
            }

            foreach (var item in plugin.db.ItemSpawns)
            {
                if (!string.IsNullOrWhiteSpace(item.GroupName))
                {
                    if (!groups.ContainsKey(item.GroupName))
                    {
                        Log.Warn("Group \"" + item.GroupName + "\" not found. Skipping item.");
                        continue;
                    }
                    groups[item.GroupName].Add(item);
                    continue;
                }
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= item.SpawnChance)
                {
                    SpawnItemFromSpawnPoint(item);
                }
            }

            foreach (var item in groups)
            {
                var group = plugin.db.Groups[item.Key];
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= group.SpawnChance)
                {
                    List<DatabaseConfigItemEntry> list = item.Value;
                    for (int i = 0; i < group.SpawnAmount; i++)
                    {
                        int idx = UnityEngine.Random.Range(0, list.Count);
                        if (UnityEngine.Random.Range(0.0f, 1.0f) <= list[idx].SpawnChance)
                        {
                            SpawnItemFromSpawnPoint(list[idx]);
                        }
                        if (!group.Stackable)
                        {
                            list.RemoveAt(idx);
                        }
                    }
                }
            }
        }

        public void SpawnItemFromSpawnPoint(DatabaseConfigItemEntry item)
        {
            foreach (var room in Map.Rooms)
            {
                if (room.Name.ToLower().Contains(plugin.db.Spawns[item.SpawnName].RoomName.ToLower()))
                {
                    SpawnPosition spos = plugin.db.Spawns[item.SpawnName].Position;
                    Vector3 pos = new Vector3(spos.X, spos.Y, spos.Z);
                    Pickup itemworld = Map.SpawnItem((ItemType)Enum.Parse(typeof(ItemType), item.ItemName, true), 0f, room.Transform.TransformPoint(pos));
                    itemworld.RefreshDurability(true, true);
                    Log.Debug("Spawning at: " + pos.ToString());
                }
            }
        }
    }
}