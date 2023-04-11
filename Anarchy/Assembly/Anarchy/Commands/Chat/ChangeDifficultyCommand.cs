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
    internal class ChangeDifficultyCommand : ChatCommand
    {
        public ChangeDifficultyCommand() : base("difficulty", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            int DIFF;
            DIFF = int.Parse(args[0]);
            if (DIFF < 0 || DIFF > 3) return false;

            FengGameManagerMKII.FGM.BasePV.RPC("SetDifficultyRPC", PhotonTargets.AllBuffered, DIFF);
            return true;
        }
    }
}