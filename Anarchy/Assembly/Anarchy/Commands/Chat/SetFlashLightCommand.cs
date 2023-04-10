using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SetFlashLightCommand : ChatCommand
    {
        public SetFlashLightCommand() : base("givelight", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            int ID = Convert.ToInt32(args[0]);
            int Toggle = Convert.ToInt32(args[1]);
            FengGameManagerMKII.FGM.BasePV.RPC("SetFlashLight", PhotonTargets.AllBuffered, ID, Toggle);
            return true;
        }
    }
}