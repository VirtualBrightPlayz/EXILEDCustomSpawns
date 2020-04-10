/*using Smod2.Commands;
using System.Collections.Generic;
using System.IO;

namespace VirtualBrightPlayz.SCPSL.CustomSpawnPositions
{
    internal class CSPCommandAddItemSpawn : ICommandHandler
    {
        private CustomSpawnPositions customSpawnPositions;

        public CSPCommandAddItemSpawn(CustomSpawnPositions customSpawnPositions)
        {
            this.customSpawnPositions = customSpawnPositions;
        }

        string ICommandHandler.GetCommandDescription()
        {
            return "Add/remove items from the spawn list";
        }

        string ICommandHandler.GetUsage()
        {
            return "addis <spawnpoint name> [(if adding or specific remove) itemid]";
        }

        string[] ICommandHandler.OnCall(ICommandSender sender, string[] args)
        {

            if (!File.Exists(FileManager.GetAppFolder() + "itemspawns.txt"))
                File.Create(FileManager.GetAppFolder() + "itemspawns.txt");
            var itemarr = new List<string>(File.ReadAllLines(FileManager.GetAppFolder() + "itemspawns.txt"));
            var itemspawns = new Dictionary<string, string>();
            foreach (var item in itemarr)
            {
                itemspawns.Add(item.Split(' ')[1], item.Split(' ')[0]);
            }
            if (args.Length == 1)
            {
                itemarr.Remove(args[0] + " " + itemspawns[args[0]]);
            }
            else if (args.Length == 2)
            {
                if (itemarr.Contains(args[0] + " " + args[1]))
                    itemarr.Remove(args[0] + " " + args[1]);
                else
                    itemarr.Add(args[0] + " " + args[1]);
            }
            else
                return new string[] { "Error." };
            File.WriteAllLines(FileManager.GetAppFolder() + "itemspawns.txt", itemarr);
            return new string[] { "Done." };
        }
    }
}*/