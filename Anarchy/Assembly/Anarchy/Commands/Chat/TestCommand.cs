using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Optimization.Caching;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    internal class TestCommand : ChatCommand
    {
        public TestCommand() : base("test", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            var obj = Pool.NetworkEnable("RCAsset/wagon1", IN_GAME_MAIN_CAMERA.MainHERO.baseT.position, new Quaternion(0f, 0f, 0f, 0f));
            return true;
        }
    }
}
