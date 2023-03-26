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
    internal class GravityCommand : ChatCommand
    {
        public GravityCommand() : base("gravity", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("SetGravityRPC", PhotonTargets.AllBuffered, float.Parse(args[0]));
            return true;
        }
    }
}