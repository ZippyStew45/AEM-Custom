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
using RC;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SpawnCannonCommand : ChatCommand
    {
        public SpawnCannonCommand() : base("cannon", false, true, true)
        {

        }

        public HERO myHero;
        public override bool Execute(string[] args)
        {
            if (!PhotonNetwork.player.Gunner) return false;
            Vector3 ppos = PhotonPlayer.MyHero().transform.position;
            Quaternion prot = PhotonPlayer.MyHero().transform.rotation;
            string settings = string.Concat(new object[]
            {
                    "photon,CannonGround,default,1,1,1,0,1,1,1,1.0,1.0,",
                    ppos.x,
                    ",",
                    ppos.y,
                    ",",
                    ppos.z,
                    ",",
                    prot.x,
                    ",",
                    prot.y,
                    ",",
                    prot.z,
                    ",",
                    prot.w
            });
            HERO.herin.SpawnCannon(settings);
            //PhotonPlayer.MyHero().gameObject.GetComponent<HERO>().BasePV.RPC("SpawnCannonRPC", PhotonTargets.MasterClient, settings);
            return true;
        }
    }
}