using Anarchy.Commands.Chat;
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
            float Size = Convert.ToSingle(args[1]);


            chatMessage = "Game Object Spawned: " + args[0];


            Vector3 ppos = PhotonPlayer.MyHero().transform.position;
            Quaternion prot = PhotonPlayer.MyHero().transform.rotation;
            FengGameManagerMKII.FGM.BasePV.RPC("SpawnPrimitiveRPC", PhotonTargets.AllBuffered, args[0], Size, false, ppos, prot);
            return true;
        }
    }
}
