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
    internal class ImpactDeathCommand : ChatCommand
    {
        public ImpactDeathCommand() : base("impact", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("SetImpactDeathRPC", PhotonTargets.AllBuffered, Convert.ToBoolean(int.Parse(args[0])), float.Parse(args[1]));
            return true;
        }
    }
}