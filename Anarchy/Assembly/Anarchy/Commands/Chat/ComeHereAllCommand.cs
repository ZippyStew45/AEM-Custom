using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class ComeHereAllCommand : ChatCommand
    {
        public ComeHereAllCommand() : base("comehereall", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            Vector3 Mypos = PhotonPlayer.MyHero().transform.position;

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                go.GetComponent<HERO>().BasePV.RPC("moveToRPC", PhotonTargets.Others, new object[] { Mypos.x, Mypos.y, Mypos.z });
            }
            return true;
        }
    }
}