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
                FengGameManagerMKII.FGM.BasePV.RPC("HideHPTitan", PhotonTargets.AllBuffered, true);
                return true;
            }
            FengGameManagerMKII.FGM.BasePV.RPC("HideHPTitan", PhotonTargets.AllBuffered, false);
            return false;
        }
    }
}