using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using Optimization.Caching;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class LightCommand : ChatCommand
    {
        public LightCommand() : base("light", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("LightRPC", PhotonTargets.AllBuffered, args[0], args[1]);
            return true;
        }
    }
}