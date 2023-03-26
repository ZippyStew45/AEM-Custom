using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class BundleLoadCommand : ChatCommand
    {
        public BundleLoadCommand() : base("bundle_load", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("LoadBundleRPC", PhotonTargets.AllBuffered, args[0]);
            return true;
        }
    }
}