using CommandSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListItems : ICommand
    {
        public string Command => "CSP_LIST_ITEMS";

        public string[] Aliases => new string[0];

        public string Description => "Lists all items.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> items = new List<string>();
            foreach (var item in Enum.GetValues(typeof(ItemType)))
            {
                items.Add(item.ToString());
            }
            response = "All Valid Items: " + items.Join();
            return true;
        }
    }
}
