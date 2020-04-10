using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions
{
    public class DatabaseConfig
    {
        public Dictionary<string, DatabaseConfigSpawnEntry> Spawns { get; set; }
        public Dictionary<string, DatabaseConfigGroupEntry> Groups { get; set; }
        public List<DatabaseConfigItemEntry> ItemSpawns { get; set; }
    }

    public class DatabaseConfigGroupEntry
    {
        //public string GroupName { get; set; }
        public float SpawnChance { get; set; }
        public int SpawnAmount { get; set; }
        public bool Stackable { get; set; }

        public override string ToString()
        {
            return "(Chance: " + SpawnChance + ", Amount: " + SpawnAmount + ", Stackable: " + Stackable + ")";
        }
    }

    public class DatabaseConfigItemEntry
    {
        public string SpawnName { get; set; }
        public string GroupName { get; set; }
        public string ItemName { get; set; }
        public float SpawnChance { get; set; }

        public override string ToString()
        {
            return "(Spawn: " + SpawnName + ", Group: " + GroupName + ", Item: " + ItemName + ", Chance: " + SpawnChance + ")";
        }
    }

    public class DatabaseConfigSpawnEntry
    {
        //public string SpawnName { get; set; }
        public string RoomName { get; set; }
        public SpawnPosition Position { get; set; }

        public override string ToString()
        {
            return "(Room: " + RoomName + ", Position: " + Position.ToString() + ")";
        }
    }

    public class SpawnPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return "(X: " + X + ", Y: " + Y + ", Z: " + Z + ")";
        }
    }
}
