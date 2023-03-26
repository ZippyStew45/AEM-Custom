using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class GiveGasCommand : ChatCommand
    {
        public GiveGasCommand() : base("givegas", false, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            PhotonPlayer target = PhotonPlayer.Find(int.Parse(args[0]));
            HERO selfHero = PhotonNetwork.player.GameObject.GetComponent<HERO>();
            if (Vector3.Distance(target.GameObject.GetComponent<HERO>().transform.position, selfHero.transform.position) <= 5 && selfHero.CurrentGas >= float.Parse(args[1]) && !target.IsLocal)
            {
                target.GameObject.GetComponent<HERO>().BasePV.RPC("giveGasRPC", target, float.Parse(args[1]));
                selfHero.ConsumeGas(float.Parse(args[1]));
                return true;
            }
            return false;
        }
    }
}
