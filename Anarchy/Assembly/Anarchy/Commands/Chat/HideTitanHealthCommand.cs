using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class HideTitanHealthCommand : ChatCommand
    {
        public HideTitanHealthCommand() : base("hidehp", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            if (args[0] == "1")
            {
                TITAN.HideHP = true;
                return true;
            }
            TITAN.HideHP = false;
            return false;
        }
    }
}