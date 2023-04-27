using Optimization.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static RCActionHelper;

namespace Anarchy.Commands.Chat
{
    internal class SpawnSupplyStationCommand : ChatCommand
    {
        public SpawnSupplyStationCommand() : base("sup", false, true, false)
        {
        }

        GameObject Supply = new GameObject();

        public override bool Execute(string[] args)
        {
            if (!PhotonNetwork.player.Wagoneer)
            {
                chatMessage = "You Need Wagon Role To Use This >:(";
                return true;
            }
            GameObject hero = PhotonPlayer.MyHero().gameObject;
            Vector3 Pos = hero.gameObject.transform.position + (hero.gameObject.transform.forward * 6f);
            Quaternion Rot = hero.gameObject.transform.rotation;
            float minDist = 15;

            if (Supply == null)
            {
                Supply = PhotonNetwork.Instantiate("aot_supply", Pos, Rot, PhotonNetwork.player.ID);
                return true;
            }
            float dist = Vector3.Distance(hero.transform.position, Supply.transform.position);
            if (dist < minDist)
            {
                PhotonNetwork.Destroy(Supply);
                return true;
            }
            else if (dist > minDist)
            {
                chatMessage = "You are too far away from the supply station.";
                return true;
            }
            return true;
        }
    }
}