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
    public class ListRoles : ICommand
    {
        public string Command => "CSP_LIST_ROLES";

        public string[] Aliases => new string[0];

        public string Description => "Lists all roles.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> rooms = new List<string>();
            foreach (var item in Enum.GetValues(typeof(RoleType)))
            {
                rooms.Add(item.ToString());
            }
            response = "All Roles by name: " + rooms.Join();
            return true;
        }
    }
}
