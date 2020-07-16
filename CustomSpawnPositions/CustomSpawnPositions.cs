using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions
{
    public class CustomSpawnPositions : Plugin<Config>
    {
        public static CustomSpawnPositions csp;

        public CSPEventHandler PLEV;
        public static string pluginDir;
        internal IDictionary<object, object> configs;
        internal DatabaseConfig db;

        public static Dictionary<string, string> roomNamesLCZ = new Dictionary<string, string>() {
            //LCZ
            { "173", "lcz_173" },
            { "classdspawn", "lcz_classdspawn" },
            { "cafe", "lcz_cafe" },
            { "chkpa", "lcz_chkpa" },
            { "chkpb", "lcz_chkpb" },
            { "914", "lcz_914" },
            { "012", "lcz_012" },
            { "toilets", "lcz_toilets" },
            { "372", "lcz_372" },
            { "armory", "lcz_armory" },
            { "airlock", "lcz_airlock" },
            { "plants", "lcz_plants" },
            { "vents", "lcz_plants" },
            { "weed", "lcz_plants" },
        };

        public static Dictionary<string, string> roomNamesHCZ = new Dictionary<string, string>() {
            //HCZ
            { "lifta", "hcz_chkpa" },
            { "liftb", "hcz_chkpb" },
            { "457", "hcz_457" },
            { "096", "hcz_457" },
            { "nuke", "hcz_nuke" },
            { "microhid", "hcz_hid" },
            { "079", "hcz_079" },
            { "049", "hcz_049" },
            { "939", "hcz_testroom" },
            { "106", "hcz_106" },
            { "servers", "hcz_servers" },
            { "ez", "hcz_ez_checkpoint" },
        };

        public override string Name => "CustomSpawnPositions";
        public override string Author => "VirtualBrightPlayz";
        public override Version Version => new Version(1, 1, 0);

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Server.RoundStarted -= PLEV.RoundStart;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= PLEV.WaitingForPlayers;
            PLEV = null;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            //string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            pluginDir = Path.Combine(Paths.Configs, "CustomSpawns");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            /*if (!File.Exists(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml")))
                File.WriteAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"), "");*/
            if (!File.Exists(Path.Combine(pluginDir, "database-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml")))
            {
                db = new DatabaseConfig()
                {
                    Groups = new Dictionary<string, DatabaseConfigGroupEntry>(),
                    ItemSpawns = new List<DatabaseConfigItemEntry>(),
                    Spawns = new Dictionary<string, DatabaseConfigSpawnEntry>()
                };
                DatabaseSave();
                //File.WriteAllText(Path.Combine(pluginDir, "database-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"), "");
            }
            DatabaseLoad();
            PLEV = new CSPEventHandler(this);
            Exiled.Events.Handlers.Server.RoundStarted += PLEV.RoundStart;
            Exiled.Events.Handlers.Server.WaitingForPlayers += PLEV.WaitingForPlayers;
        }

        /*public void ConfigLoad()
        {
            string data = File.ReadAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"));
            var des = new DeserializerBuilder().Build();
            configs = (IDictionary<object, object>)des.Deserialize<object>(data);
            autoSave = configs.GetBool("csp_auto_save", true);
            replacePlayerSpawns = configs.GetBool("csp_replace_player_spawns", false);
        }*/

        /*public void ConfigSave()
        {
            Log.Info("Saving config...");
            var ser = new SerializerBuilder().Build();
            File.WriteAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"), ser.Serialize(configs));
            Log.Info("Config saved!");
        }*/

        public void DatabaseSave()
        {
            Log.Info("Saving database...");
            var ser = new SerializerBuilder().Build();
            File.WriteAllText(Path.Combine(pluginDir, "database-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"), ser.Serialize(db));
            Log.Info("Database saved!");
        }

        public void DatabaseLoad()
        {
            string data = File.ReadAllText(Path.Combine(pluginDir, "database-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"));
            var des = new DeserializerBuilder().Build();
            db = des.Deserialize<DatabaseConfig>(data);
            if (db.Spawns == null)
                db.Spawns = new Dictionary<string, DatabaseConfigSpawnEntry>();
            if (db.Groups == null)
                db.Groups = new Dictionary<string, DatabaseConfigGroupEntry>();
            if (db.PlayerSpawns == null)
                db.PlayerSpawns = new List<DatabaseConfigPlayerEntry>();
            if (db.ItemSpawns == null)
                db.ItemSpawns = new List<DatabaseConfigItemEntry>();
            DatabaseSave();
        }
    }
}
