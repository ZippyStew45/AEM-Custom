using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;
using Anarchy;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class CommonVDCommand : ChatCommand
    {
        public CommonVDCommand() : base("commonvd", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            TITAN.commonVD = float.Parse(args[0]);
            FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, $"Common View Distance set to: {args[0]}", "");
            return true;
        }//User.WebHook.Value.ToString()
    }
}