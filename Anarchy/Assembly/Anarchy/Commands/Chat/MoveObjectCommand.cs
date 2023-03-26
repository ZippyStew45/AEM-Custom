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
    internal class MoveObjectCommand : ChatCommand
    {
        public MoveObjectCommand() : base("moveobject", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("MoveObjectRPC", PhotonTargets.AllBuffered, args[0], new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])), new Quaternion(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7])));
            return true;
        }
    }
}