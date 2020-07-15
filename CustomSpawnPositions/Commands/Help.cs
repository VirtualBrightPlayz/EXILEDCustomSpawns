using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Help : ICommand
    {
        public string Command => "CSP_HELP";

        public string[] Aliases => new string[0];

        public string Description => "Help command for CSP.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "CustomSpawnPositions by VirtualBrightPlayz/Brian Zulch.\nCommands:" + "\nCSP_SPAWN_ADD" + "\nCSP_SPAWN_LIST" + "\nCSP_SPAWN_DEL" + "\nCSP_GROUP_ADD" + "\nCSP_GROUP_LIST" + "\nCSP_GROUP_DEL" + "\nCSP_ITEM_ADD" + "\nCSP_ITEM_DEL_ALL_YES" + "\nCSP_PLAYER_ADD" + "\nCSP_PLAYER_DEL_ALL_YES" + "\nCSP_LIST_ITEMS" + "\nCSP_LIST_ROOMS" + "\nCSP_LIST_ROLES";
            return true;
        }
    }
}
