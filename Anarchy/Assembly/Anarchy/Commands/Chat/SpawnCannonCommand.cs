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
        public SpawnCannonCommand() : base("cannon", true, true, true)
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
            Photon.MonoBehaviour hero = null;
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Stop)
            {

                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                {
                    if (FengGameManagerMKII.Heroes.Count > 0)
                    {
                        hero = FengGameManagerMKII.Heroes[0];
                    }
                }
                else if (PhotonNetwork.player.IsTitan)
                {
                    hero = PhotonNetwork.player.GetTitan();
                }
                else
                {
                    hero = PhotonNetwork.player.GetHero();
                }
            }
            hero.gameObject.GetComponent<HERO>().BasePV.RPC("SpawnCannonRPC", PhotonTargets.MasterClient, settings);
            return true;
        }
    }
}