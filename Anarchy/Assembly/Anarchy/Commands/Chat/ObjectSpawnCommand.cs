using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class ObjectSpawnCommand : ChatCommand
    {
        public ObjectSpawnCommand() : base("object_spawn", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("SpawnObjectRPC", PhotonTargets.AllBuffered, args[0], args[1], new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4])), new Quaternion(float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]), float.Parse(args[8])));
            return true;
        }
    }
}