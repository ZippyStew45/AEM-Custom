using Anarchy.Commands.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class ShakeScreenCommand : ChatCommand
    {
        public ShakeScreenCommand() : base("shakescreen", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("ShakeScreenRPC", PhotonTargets.All, new object[] { float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]) });
            return true;
        }
    }
}