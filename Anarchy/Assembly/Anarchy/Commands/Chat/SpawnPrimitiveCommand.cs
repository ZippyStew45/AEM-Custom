using Anarchy.Commands.Chat;
using AoTTG.EMAdditions;
using Optimization.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SpawnPrimitiveCommand : ChatCommand
    {
        public SpawnPrimitiveCommand() : base("obj", false, true, true)
        {

        }
        
        public override bool Execute(string[] args)
        {
            if (!PhotonNetwork.player.Builder)
                return false;
            chatMessage = "Game Object Spawned: " + args[0];
            ObjControll.SavedOBJ = args[0];
            ObjControll.RespawnOBJ(args[0]);
            return true;
        }
    }
}
