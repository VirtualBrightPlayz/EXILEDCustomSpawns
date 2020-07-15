using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
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
                Pickup itemworld = type.Spawn(0f, pos);
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
                    Pickup itemworld = type.Spawn(0f, room.Transform.TransformPoint(pos));
                    itemworld.RefreshDurability(true, true);
                    Log.Debug("Spawning " + type.ToString() + " at: " + pos.ToString());
                }
            }
        }

        public void SpawnGameObjFromSpawnPoint(GameObject item, string spawn, string tag)
        {
            if (plugin.db.Spawns[spawn].RoomName.ToLower().Equals("none"))
            {
                if (plugin.Config.csp_replace_player_spawns && !replacedPlayerSpawns.Contains(tag))
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
            if (found > 0 && plugin.Config.csp_replace_player_spawns && !replacedPlayerSpawns.Contains(tag))
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