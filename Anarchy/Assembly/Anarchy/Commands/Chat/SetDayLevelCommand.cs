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
            if (args[1] != null && args[2] != null)
            {
                float g = Convert.ToSingle(args[1]);
                float b = Convert.ToSingle(args[2]);
                FengGameManagerMKII.FGM.BasePV.RPC("SetDayLevel", PhotonTargets.AllBuffered, r, g, b);
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("SetDayLevel", PhotonTargets.AllBuffered, r, r, r);
            }
            return true;
        }
    }
}