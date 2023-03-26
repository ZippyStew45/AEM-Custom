using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SetHSCommand : ChatCommand
    {
        public SetHSCommand() : base("seths", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            var p = PhotonPlayer.Find(int.Parse(args[0]));
            p.GameObject.GetComponent<HERO>().BasePV.RPC("sethsRPC", p, float.Parse(args[1]));
            return true;
        }
    }
}