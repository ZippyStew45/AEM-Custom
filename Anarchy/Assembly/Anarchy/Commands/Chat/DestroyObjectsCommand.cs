using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using Anarchy.Network.Discord.SDK;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class DestroyObjectsCommand : ChatCommand
    {
        public DestroyObjectsCommand() : base("destroyobjects", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            if (args.Length == 0) return false;
            FengGameManagerMKII.FGM.BasePV.RPC("DestroyObjectsRPC", PhotonTargets.AllBuffered, new object[]{args});
            return true;
        }
    }
}