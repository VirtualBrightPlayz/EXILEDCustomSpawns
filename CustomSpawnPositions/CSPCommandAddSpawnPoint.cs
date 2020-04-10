/*using Smod2.API;
using Smod2.Commands;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions
{
    internal class CSPCommandAddSpawnPoint : ICommandHandler
    {
        private CustomSpawnPositions customSpawnPositions;

        public CSPCommandAddSpawnPoint(CustomSpawnPositions customSpawnPositions)
        {
            this.customSpawnPositions = customSpawnPositions;
        }

        string ICommandHandler.GetCommandDescription()
        {
            return "Add a spawnpoint to the configs.";
        }

        string ICommandHandler.GetUsage()
        {
            return "addsp <name of new sp | name of sp to delete>";
        }

        string[] ICommandHandler.OnCall(ICommandSender sender, string[] args)
        {
            if (sender as Player == null && false)
                return new string[] { "You must be a player!" };
            else if (args.Length >= 1)
            {
                if (args[0].Trim().ToLower().Equals("debug"))
                {
                    //FileManager.AppendFile(args[0], "spawnpoint_data.txt");
                    var arr2 = new List<string>(File.ReadAllLines(FileManager.GetAppFolder() + "spawnpoint_db.txt"));
                    var lczrooms = GameObject.Find("LightRooms").transform;
                    var hczrooms = GameObject.Find("HeavyRooms").transform;
                    var ezrooms = GameObject.Find("EntranceRooms").transform;
                    List<string> arr = new List<string>();
                    customSpawnPositions.Info("LightRooms");
                    for (int i = 0; i < lczrooms.childCount; i++)
                    {
                        var room = lczrooms.GetChild(i);
                        customSpawnPositions.Info(room.name);
                        int j = 0;
                        if (room.name.ToLower().Contains(arr2[j].Split(' ')[0]))
                        {
                            Vector3 pos1 = new Vector3(float.Parse(arr2[j].Split(' ')[2]), float.Parse(arr2[j].Split(' ')[3]), float.Parse(arr2[j].Split(' ')[4]));
                            Pickup.inv.SetPickup((int)ItemType.MICROHID, -4.65664672E+11f, room.TransformPoint(pos1), Quaternion.identity, 0, 0, 0);
                        }
                        //arr.Add(room.name);
                        //room.name;
                    }
                    customSpawnPositions.Info("HeavyRooms");
                    for (int i = 0; i < hczrooms.childCount; i++)
                    {
                        var room = hczrooms.GetChild(i);
                        customSpawnPositions.Info(room.name);
                    }
                    customSpawnPositions.Info("EntranceRooms");
                    for (int i = 0; i < ezrooms.childCount; i++)
                    {
                        var room = ezrooms.GetChild(i);
                        customSpawnPositions.Info(room.name);
                    }
                    return arr.ToArray();
                }
                else if (args[0].Trim().ToLower().Equals("list"))
                {
                    var str = new List<string>();
                    str.Add("LCZ:");
                    foreach (var item in CustomSpawnPositions.roomNamesLCZ)
                    {
                        str.Add(item.Key);
                    }
                    str.Add("HCZ:");
                    foreach (var item in CustomSpawnPositions.roomNamesHCZ)
                    {
                        str.Add(item.Key);
                    }
                    return str.ToArray();
                }
                else if (args[0].Trim().ToLower().Equals("listsp"))
                {
                    if (!File.Exists(FileManager.GetAppFolder() + "spawnpoint_db.txt"))
                        File.Create(FileManager.GetAppFolder() + "spawnpoint_db.txt");
                    var arr = new List<string>(File.ReadAllLines(FileManager.GetAppFolder() + "spawnpoint_db.txt"));
                    return arr.ToArray();
                }
                else if (sender as Player != null && args.Length >= 2)
                {
                    Player player = sender as Player;
                    Vector3 playerpos = new Vector3(player.GetPosition().x, player.GetPosition().y, player.GetPosition().z);
                    if (!File.Exists(FileManager.GetAppFolder() + "spawnpoint_db.txt"))
                        File.Create(FileManager.GetAppFolder() + "spawnpoint_db.txt");
                    var arr = new List<string>(File.ReadAllLines(FileManager.GetAppFolder() + "spawnpoint_db.txt"));
                    foreach (string str in arr)
                    {
                        if (str.Split(' ')[1].Equals(args[1]))
                            return new string[] { "Error, name taken." };
                    }
                    var lczrooms = GameObject.Find("LightRooms").transform;
                    var hczrooms = GameObject.Find("HeavyRooms").transform;
                    var ezrooms = GameObject.Find("EntranceRooms").transform;
                    for (int i = 0; i < lczrooms.childCount; i++)
                    {
                        var room = lczrooms.GetChild(i);
                        Vector3 pos1 = room.position;
                        Vector3 pos2 = playerpos - pos1;
                        pos2 = room.InverseTransformPoint(playerpos);
                        if (CustomSpawnPositions.roomNamesLCZ.ContainsKey(args[0].ToLower().Trim()) && room.name.ToLower().Contains(CustomSpawnPositions.roomNamesLCZ[args[0].ToLower().Trim()]))
                        {
                            arr.Add(args[0].ToLower().Trim() + " " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //this is old, before the dict.
                        /*
                        //173 spawn
                        if (room.name.ToLower().Contains("173") && args[0].ToLower().Trim().Contains("173"))
                        {
                            arr.Add("173 " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //dclass spawn
                        if (room.name.ToLower().Contains("classdspawn") && args[0].ToLower().Trim().Contains("dspawn"))
                        {
                            arr.Add("classdspawn " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //cafe/pc room
                        if (room.name.ToLower().Contains("cafe") && args[0].ToLower().Trim().Contains("cafe"))
                        {
                            arr.Add("cafe " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //checkpoint a
                        if (room.name.ToLower().Contains("chkpa") && args[0].ToLower().Trim().Contains("chkpta"))
                        {
                            arr.Add("chkpa " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //checkpoint b
                        if (room.name.ToLower().Contains("chkpb") && args[0].ToLower().Trim().Contains("chkptb"))
                        {
                            arr.Add("chkpb " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //914
                        if (room.name.ToLower().Contains("914") && args[0].ToLower().Trim().Contains("914"))
                        {
                            arr.Add("914 " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //012
                        if (room.name.ToLower().Contains("012") && args[0].ToLower().Trim().Contains("012"))
                        {
                            arr.Add("012 " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //wc
                        if (room.name.ToLower().Contains("toilets") && args[0].ToLower().Trim().Contains("wc"))
                        {
                            arr.Add("toilets " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                        //372
                        if (room.name.ToLower().Contains("372") && args[0].ToLower().Trim().Contains("372"))
                        {
                            arr.Add("372 " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }* /
                    }
                    for (int i = 0; i < hczrooms.childCount; i++)
                    {
                        var room = hczrooms.GetChild(i);
                        Vector3 pos1 = room.position;
                        Vector3 pos2 = playerpos - pos1;
                        pos2 = room.InverseTransformPoint(playerpos);
                        if (CustomSpawnPositions.roomNamesHCZ.ContainsKey(args[0].ToLower().Trim()) && room.name.ToLower().Contains(CustomSpawnPositions.roomNamesHCZ[args[0].ToLower().Trim()]))
                        {
                            arr.Add(args[0].ToLower().Trim() + " " + args[1] + " " + pos2.x + " " + pos2.y + " " + pos2.z);
                        }
                    }
                    File.WriteAllLines(FileManager.GetAppFolder() + "spawnpoint_db.txt", arr);
                    FileManager.GetAppFolder();
                    return arr.ToArray();
                }
            }
            return new string[] { "Error." };
        }
    }
}
/*
[21:45:14] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] DoorsLight
[21:45:14] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ClassDSpawn (1)
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_173
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Cafe (15)
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_#ChkpB (22)
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_@ChkpA (21)
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Armory
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_372 (18)
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Toilets
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_914 (14)
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_012 (12)

[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] DoorsHeavy
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_@ChkpA
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_#ChkpB
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_!Nuke
[21:45:15] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_457
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Hid
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_079
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_$049
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Testroom
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_106
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Servers
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Checkpoint
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_*&*GateB
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_*&*GateA
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Smallrooms1
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Chef
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Cafeteria
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_PCs
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_upstairs
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Smallrooms2
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_PCs_small
[21:45:16] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Intercom
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] EntranceDoors
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_CollapsedTunnel (1)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Endoof (1)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (1)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (2)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (3)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (4)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (5)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (6)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (7)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (1)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (2)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (3)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (4)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (5)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (6)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (7)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (8)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (9)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (10)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (11)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (12)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (13)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (14)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (1)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (2)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (3)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (4)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (5)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (6)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (7)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (8)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (9)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (10)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (11)
[21:45:17] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (12)
DoorsLight
Root_ClassDSpawn (1)
Root_Straight
Root_Vent
Root_Crossing
Root_Curve
Root_Airlock
Root_Troom
Root_173
Root_Cafe (15)
Root_#ChkpB (22)
Root_@ChkpA (21)
Root_Armory
Root_372 (18)
Root_Toilets
Root_914 (14)
Root_012 (12)
Root_Vent (1)
Root_Vent (2)
Root_Vent (3)
Root_Vent (4)
Root_Troom (1)
Root_Troom (2)
Root_Troom (3)
Root_Troom (4)
Root_Troom (5)
Root_Troom (6)
Root_Troom (7)
Root_Troom (8)
Root_Troom (9)
Root_Troom (10)
Root_Troom (11)
Root_Straight (1)
Root_Straight (2)
Root_Straight (3)
Root_Straight (4)
Root_Straight (5)
Root_Airlock (1)
Root_Crossing (1)
Root_Crossing (2)
Root_Crossing (3)
Root_Crossing (4)
Root_Crossing (5)
Root_Crossing (6)
Root_Crossing (7)
Root_Curve (1)
Root_Curve (2)
Root_Curve (3)
Root_Curve (4)
Root_Curve (5)
Root_Curve (6)
Root_Curve (7)
Root_Curve (8)
Root_Curve (9)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] LightRooms
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] DoorsLight
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ClassDSpawn (1)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Vent
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Airlock
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_173
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Cafe (15)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_#ChkpB (22)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_@ChkpA (21)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Armory
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_372 (18)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Toilets
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_914 (14)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_012 (12)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Vent (1)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Vent (2)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Vent (3)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Vent (4)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (1)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (2)
[22:24:39] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (8)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (9)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (10)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Troom (11)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Airlock (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (8)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (9)

[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] HeavyRooms
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Map_PD_Main
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] DoorsHeavy
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_@ChkpA
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_#ChkpB
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_!Nuke
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_457
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Tesla
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Hid
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_079
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_$049
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Testroom
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3ar
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_106
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Servers
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Checkpoint
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Crossing (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Tesla (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Tesla (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Tesla (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Tesla (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (8)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (9)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (10)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (11)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (12)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Room3 (13)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (8)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (9)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (10)

[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] EntranceRooms
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_CollapsedTunnel
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_*&*GateB
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_*&*GateA
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Endoof
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Shelter
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Smallrooms1
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Chef
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Cafeteria
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_PCs
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_upstairs
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Smallrooms2
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_PCs_small
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Intercom
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] EntranceDoors
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_CollapsedTunnel (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Endoof (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Straight (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (8)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (9)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (10)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (11)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (12)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (13)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_ Crossing (14)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (1)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (2)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (3)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (4)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (5)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (6)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (7)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (8)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (9)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (10)
[22:24:40] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (11)
[22:24:41] [INFO] [virtualbrightplayz.scpsl.customspawnpositions] Root_Curve (12)
*/
