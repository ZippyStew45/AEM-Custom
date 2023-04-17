using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using System.Runtime;
using Anarchy.IO;
using Mod;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class RunCMDListCommand : ChatCommand
    {
        public RunCMDListCommand() : base("cmdlist", false, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            string Path = Environment.CurrentDirectory + "\\AoTTG_Data\\CommandLists\\";
            string message = string.Join(" ", args, 0, args.Length);
            if (!System.IO.File.Exists($"{Path}{message}.txt"))
            {
                chatMessage = $"file \"{message}.txt\" does not exist!";
                return false;
            }
            ;
            FengGameManagerMKII.FGM.StartCoroutine(CommandList.RunCmdtest($"{Path}{message}.txt"));
            return true;
        }
    }
}
