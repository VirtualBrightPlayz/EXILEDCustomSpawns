using CommandSystem;
using Exiled.API.Features;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListRooms : ICommand
    {
        public string Command => "CSP_LIST_ROOMS";

        public string[] Aliases => new string[0];

        public string Description => "Lists all rooms in the current map by name.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> rooms = new List<string>();
            foreach (var item in Map.Rooms)
            {
                rooms.Add(item.Name);
            }
            response = "All Rooms in current map: " + rooms.Join() + "\nUse the room name! Not the number! (use \"ClassDSpawn\" not \"ClassDSpawn (22)\")";
            return true;
        }
    }
}
