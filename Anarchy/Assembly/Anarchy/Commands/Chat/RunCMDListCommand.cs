using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.IO;
using Mod;
using UnityEngine;

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
            if (!Directory.Exists($"{Path}{args[0]}.txt"))
            {
                chatMessage = $"file {args[0]} does not exist!";
            }

            CommandList.RunCmdtest($"{Path}{args[0]}.txt");
            return true;
        }
    }
}
