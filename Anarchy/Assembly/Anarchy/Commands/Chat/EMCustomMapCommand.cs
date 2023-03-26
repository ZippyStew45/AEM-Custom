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
    internal class EMCustomMapCommand : ChatCommand
    {
        public EMCustomMapCommand() : base("emcustommap", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            if (args.Length != 9) return false;
            FengGameManagerMKII.FGM.BasePV.RPC("EMCustomMapRPC", PhotonTargets.AllBuffered, args[0], args[1], new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4])), new Quaternion(float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]), float.Parse(args[8])));
            return true;
        }
    }
}