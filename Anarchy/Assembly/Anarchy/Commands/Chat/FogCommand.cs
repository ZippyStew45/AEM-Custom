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
    internal class FogCommand : ChatCommand
    {
        public FogCommand() : base("fog", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("FogRPC", PhotonTargets.AllBuffered, args[0], args[1]);
            return true;
        }
    }
}