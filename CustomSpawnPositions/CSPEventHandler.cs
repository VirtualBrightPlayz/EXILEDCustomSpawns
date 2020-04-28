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
        private List<string> replacedPlayerSpawns = new List<string>();

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
                    ev.Allow = false;
                    ev.Sender.RAMessage("CustomSpawns config reloaded!", pluginName: plugin.getName);
                }
            }
            if (player.CheckPermission("customspawns.manage") && player != PlayerManager.localPlayer.GetPlayer())
            {
                if (args[0].ToUpper().Equals("CSP_SPAWN_ADD"))
                {
                    if (args.Length == 3 || (args.Length == 4 && !bool.Parse(args[3])))
                    {
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn already exists!", pluginName: plugin.getName);
                            return;
                        }
                        if (args[2].ToLower().Equals("none"))
                        {
                            Vector3 pos2 = player.GetPosition();
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
                            Map.SpawnItem(ItemType.Coin, 0f, pos2);
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn added.", pluginName: plugin.getName);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            return;
                        }
                        Room final = null;
                        float dist = float.MaxValue;
                        foreach (var room in Map.Rooms)
                        {
                            if ((room.Name.ToLower().StartsWith(args[2].ToLower()) || args[2].ToLower().Equals("close")) && Vector3.Distance(room.Transform.position, player.GetPosition()) < dist)
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
                                RoomName = /*args[2].ToLower().Equals("close") ?*/ final.Name.Split(' ')[0] /*: args[2].ToLower()*/,
                                Position = new SpawnPosition()
                                {
                                    X = pos2.x,
                                    Y = pos2.y,
                                    Z = pos2.z
                                }
                            };
                            plugin.db.Spawns.Add(spawnName, entry);
                            Map.SpawnItem(ItemType.Coin, 0f, pos2);
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn added.", pluginName: plugin.getName);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            return;
                        }
                        ev.Allow = false;
                        ev.Sender.RAMessage("Room not found! Use \"CLOSE\" if you want to use the closest room. Use \"NONE\" if it is room independent (ex. surface zone).", pluginName: plugin.getName);
                        return;
                    }
                    else if (args.Length == 4 && bool.Parse(args[3]))
                    {
                        //player.plyMovementSync.Rotations;
                        Vector3 dir = player.GetComponent<Scp049PlayerScript>().plyCam.transform.forward; //new Vector3(player.plyMovementSync.Rotations.x, player.plyMovementSync.Rotations.y, 0f);
                        RaycastHit output;
                        if (!Physics.Raycast(player.GetComponent<Scp049PlayerScript>().plyCam.transform.position, dir, out output, 100f))
                        {
                            ev.Allow = false;
                            ev.Sender.RAMessage("Look at something, not nothing!", pluginName: plugin.getName);
                            return;
                        }
                        Vector3 pos = output.point;
                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn already exists!", pluginName: plugin.getName);
                            return;
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
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn added.", pluginName: plugin.getName);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            return;
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
                            Map.SpawnItem(ItemType.Coin, 0f, pos);
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn added.", pluginName: plugin.getName);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            return;
                        }
                        ev.Allow = false;
                        ev.Sender.RAMessage("Room not found! Use \"CLOSE\" if you want to use the closest room. Use \"NONE\" if it is room independent (ex. surface zone).", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = false;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_SPAWN_ADD <SpawnName> <RoomName|CLOSE|NONE> [Looking(true|false)]", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_SPAWN_LIST"))
                {
                    ev.Allow = false;
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
                            ev.Allow = false;
                            ev.Sender.RAMessage("Spawn removed.", pluginName: plugin.getName);
                            return;
                        }
                    }
                    ev.Allow = false;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_SPAWN_DEL <SpawnName>", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_LIST_ITEMS"))
                {
                    ev.Allow = false;
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
                    ev.Allow = false;
                    List<string> rooms = new List<string>();
                    foreach (var item in Map.Rooms)
                    {
                        rooms.Add(item.Name);
                    }
                    ev.Sender.RAMessage("All Rooms in current map: " + rooms.Join(), pluginName: plugin.getName);
                    ev.Sender.RAMessage("Use the room name! Not the number! (use \"ClassDSpawn\" not \"ClassDSpawn (22)\")", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_LIST_ROLES"))
                {
                    ev.Allow = false;
                    List<string> rooms = new List<string>();
                    foreach (var item in Enum.GetValues(typeof(RoleType)))
                    {
                        rooms.Add(item.ToString());
                    }
                    ev.Sender.RAMessage("All Roles by name: " + rooms.Join(), pluginName: plugin.getName);
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
                                Stackable = args.Length >= 5 ? bool.Parse(args[4]) : false
                            };
                            plugin.db.Groups.Add(args[1], entry);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = false;
                            ev.Sender.RAMessage("Group added.", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = false;
                        ev.Sender.RAMessage("Group already exists!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = false;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_GROUP_ADD <GroupName> [SpawnChance (0.0-1.0)] [SpawnAmount (non-decimal number)] [Stackable (true|false)]", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_GROUP_LIST"))
                {
                    ev.Allow = false;
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
                            ev.Allow = false;
                            ev.Sender.RAMessage("Group removed.", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = false;
                        ev.Sender.RAMessage("Group does not exist!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = false;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_GROUP_DEL <GroupName>", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_ITEM_ADD"))
                {
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
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = false;
                            ev.Sender.RAMessage("Item Spawn added!", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = false;
                        ev.Sender.RAMessage("Spawn or Group not found!", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = false;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_ITEM_ADD <SpawnName> <ItemName> [SpawnChance (0.0-1.0)] [GroupName]", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_ITEM_DEL_ALL_YES"))
                {
                    plugin.db.ItemSpawns.Clear();
                    if (plugin.autoSave)
                        plugin.DatabaseSave();
                    ev.Allow = false;
                    ev.Sender.RAMessage("Item Spawns cleared.", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_PLAYER_ADD"))
                {
                    if (args.Length == 3)
                    {

                        string spawnName = args[1].ToLower().Replace('.', '_');
                        if (plugin.db.Spawns.ContainsKey(spawnName))
                        {
                            RoleType type = args[2].ToLower().Equals("current") ? player.characterClassManager.CurClass : (RoleType)Enum.Parse(typeof(RoleType), args[2], true);
                            DatabaseConfigPlayerEntry entry = new DatabaseConfigPlayerEntry()
                            {
                                ClassName = type.ToString(),
                                SpawnName = spawnName
                            };
                            plugin.db.PlayerSpawns.Add(entry);
                            if (plugin.autoSave)
                                plugin.DatabaseSave();
                            ev.Allow = false;
                            ev.Sender.RAMessage("Player Spawn added!", pluginName: plugin.getName);
                            return;
                        }
                        ev.Allow = false;
                        ev.Sender.RAMessage("Spawn not found! Use \"CURRENT\" for the current player role.", pluginName: plugin.getName);
                        return;
                    }
                    ev.Allow = false;
                    ev.Sender.RAMessage("Invalid! Usage: CSP_PLAYER_ADD <SpawnName> <RoleType|CURRENT>", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_PLAYER_DEL_ALL_YES"))
                {
                    plugin.db.PlayerSpawns.Clear();
                    if (plugin.autoSave)
                        plugin.DatabaseSave();
                    ev.Allow = false;
                    ev.Sender.RAMessage("Player Spawns cleared.", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_HELP"))
                {
                    ev.Allow = false;
                    ev.Sender.RAMessage("CustomSpawnPositions by VirtualBrightPlayz/Brian Zulch.\nCommands:" + "\nCSP_SPAWN_ADD" + "\nCSP_SPAWN_LIST" + "\nCSP_SPAWN_DEL" + "\nCSP_GROUP_ADD" + "\nCSP_GROUP_LIST" + "\nCSP_GROUP_DEL" + "\nCSP_ITEM_ADD" + "\nCSP_ITEM_DEL_ALL_YES" + "\nCSP_PLAYER_ADD" + "\nCSP_PLAYER_DEL_ALL_YES" + "\nCSP_LIST_ITEMS" + "\nCSP_LIST_ROOMS" + "\nCSP_LIST_ROLES", pluginName: plugin.getName);
                    return;
                }

                if (args[0].ToUpper().Equals("CSP_HELP"))
                {
                    plugin.DatabaseSave();
                    ev.Allow = false;
                    ev.Sender.RAMessage("Database saved.", pluginName: plugin.getName);
                    return;
                }
            }
        }

        internal void WaitingForPlayers()
        {
            replacedPlayerSpawns = new List<string>();
            foreach (var item in plugin.db.PlayerSpawns)
            {
                if (plugin.db.Spawns.ContainsKey(item.SpawnName))
                {
                    RoleType type = (RoleType)Enum.Parse(typeof(RoleType), item.ClassName);
                    switch (type)
                    {
                        case RoleType.None:
                            break;
                        case RoleType.Scp173:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_173");
                            break;
                        case RoleType.ClassD:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_CDP");
                            break;
                        case RoleType.Spectator:
                            break;
                        case RoleType.Scp106:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_106");
                            break;
                        case RoleType.NtfScientist:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_MTF");
                            break;
                        case RoleType.Scp049:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_049");
                            break;
                        case RoleType.Scientist:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_RSC");
                            break;
                        case RoleType.Scp079:
                            //SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_079");
                            break;
                        case RoleType.ChaosInsurgency:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_CI");
                            break;
                        case RoleType.Scp096:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_096");
                            break;
                        case RoleType.Scp0492:
                            break;
                        case RoleType.NtfLieutenant:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_MTF");
                            break;
                        case RoleType.NtfCommander:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_MTF");
                            break;
                        case RoleType.NtfCadet:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_MTF");
                            break;
                        case RoleType.Tutorial:
                            break;
                        case RoleType.FacilityGuard:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_GUARD");
                            break;
                        case RoleType.Scp93953:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_939");
                            break;
                        case RoleType.Scp93989:
                            SpawnGameObjFromSpawnPoint(new GameObject(), item.SpawnName, "SP_939");
                            break;
                    }
                }
                else
                {
                    Log.Warn("Spawn \"" + item + "\" not found. Skipping item.");
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
            if (plugin.db.Spawns[item.SpawnName].RoomName.ToLower().Equals("none"))
            {
                SpawnPosition spos = plugin.db.Spawns[item.SpawnName].Position;
                Vector3 pos = new Vector3(spos.X, spos.Y, spos.Z);
                ItemType type = (ItemType)Enum.Parse(typeof(ItemType), item.ItemName, true);
                Pickup itemworld = Map.SpawnItem(type, 0f, pos);
                itemworld.RefreshDurability(true, true);
                Log.Debug("Spawning " + type.ToString() + " at: " + pos.ToString());
                return;
            }
            foreach (var room in Map.Rooms)
            {
                if (room.Name.ToLower().Contains(plugin.db.Spawns[item.SpawnName].RoomName.ToLower()))
                {
                    SpawnPosition spos = plugin.db.Spawns[item.SpawnName].Position;
                    Vector3 pos = new Vector3(spos.X, spos.Y, spos.Z);
                    ItemType type = (ItemType)Enum.Parse(typeof(ItemType), item.ItemName, true);
                    Pickup itemworld = Map.SpawnItem(type, 0f, room.Transform.TransformPoint(pos));
                    itemworld.RefreshDurability(true, true);
                    Log.Debug("Spawning " + type.ToString() + " at: " + pos.ToString());
                }
            }
        }

        public void SpawnGameObjFromSpawnPoint(GameObject item, string spawn, string tag)
        {
            if (plugin.db.Spawns[spawn].RoomName.ToLower().Equals("none"))
            {
                if (plugin.replacePlayerSpawns && !replacedPlayerSpawns.Contains(tag))
                {
                    GameObject[] spawns = GameObject.FindGameObjectsWithTag(tag);
                    for (int i = 0; i < spawns.Length; i++)
                    {
                        GameObject.Destroy(spawns[i]);
                    }
                    replacedPlayerSpawns.Add(tag);
                }
                SpawnPosition spos = plugin.db.Spawns[spawn].Position;
                Vector3 pos = new Vector3(spos.X, spos.Y, spos.Z);
                var go = GameObject.Instantiate(item);
                go.transform.position = pos;
                go.tag = tag;
                Log.Debug("Spawning at: " + pos.ToString());
                return;
            }
            int found = 0;
            foreach (var room in Map.Rooms)
            {
                if (room.Name.ToLower().Contains(plugin.db.Spawns[spawn].RoomName.ToLower()))
                {
                    SpawnPosition spos = plugin.db.Spawns[spawn].Position;
                    Vector3 pos = new Vector3(spos.X, spos.Y, spos.Z);
                    var go = GameObject.Instantiate(item);
                    go.transform.position = room.Transform.TransformPoint(pos);
                    go.tag = tag;
                    Log.Debug("Spawning at: " + pos.ToString());
                    found++;
                }
            }
            if (found > 0 && plugin.replacePlayerSpawns && !replacedPlayerSpawns.Contains(tag))
            {
                GameObject[] spawns = GameObject.FindGameObjectsWithTag(tag);
                for (int i = 0; i < spawns.Length; i++)
                {
                    GameObject.Destroy(spawns[i]);
                }
                replacedPlayerSpawns.Add(tag);
            }
            foreach (var room in Map.Rooms)
            {
                if (room.Name.ToLower().Contains(plugin.db.Spawns[spawn].RoomName.ToLower()))
                {
                    SpawnPosition spos = plugin.db.Spawns[spawn].Position;
                    Vector3 pos = new Vector3(spos.X, spos.Y, spos.Z);
                    var go = GameObject.Instantiate(item);
                    go.transform.position = room.Transform.TransformPoint(pos);
                    go.tag = tag;
                    Log.Debug("Spawning at: " + pos.ToString());
                }
            }
        }
    }
}