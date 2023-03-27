using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;
using Anarchy;
using Optimization.Caching;
using System.ComponentModel;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SpawnCannonCommand : ChatCommand
    {
        public SpawnCannonCommand() : base("cannon", true, true, true)
        {

        }

        public override bool Execute(string[] args)
        {
            GameObject go = Pool.NetworkEnable("RCAsset/CannonGround", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            return true;
        }
    }
}