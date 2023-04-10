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
    internal class SetDayLevelCommand : ChatCommand
    {
        public SetDayLevelCommand() : base("setdaylightcolor", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            float r = Convert.ToSingle(args[0]);
            float g = Convert.ToSingle(args[1]);
            float b = Convert.ToSingle(args[3]);
            FengGameManagerMKII.FGM.BasePV.RPC("SetDayLevel", PhotonTargets.AllBuffered, r, g, b);
            return true;
        }
    }
}