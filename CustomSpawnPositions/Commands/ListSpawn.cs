using CommandSystem;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListSpawn : ICommand
    {
        public string Command => "CSP_SPAWN_LIST";

        public string[] Aliases => new string[0];

        public string Description => "Lists all spawns.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> items = new List<string>();
            foreach (var item in CustomSpawnPositions.csp.db.Spawns)
            {
                items.Add(item.Key + ": " + item.Value.ToString());
            }
            response = "All Spawn Points: " + items.Join();
            return true;
        }
    }
}
