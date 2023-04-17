using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;
using Anarchy;
using Optimization.Caching;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SpawnFX : ChatCommand
    {
        public SpawnFX() : base("fx", true, true, true)
        {

        }

        public override bool Execute(string[] args)
        {
            Vector3 pos = Vector3.zero;
            Quaternion quat = Quaternion.identity;

            pos.x = Convert.ToSingle(args[1]);
            pos.y = Convert.ToSingle(args[2]);
            pos.z = Convert.ToSingle(args[3]);

            quat.x = Convert.ToSingle(args[4]);
            quat.y = Convert.ToSingle(args[5]);
            quat.z = Convert.ToSingle(args[6]);
            quat.w = Convert.ToSingle(args[7]);

            PhotonNetwork.Instantiate(args[0], pos, quat, 0);
            return true;
        }
    }
}