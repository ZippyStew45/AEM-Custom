using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class ObjectLoadCommand : ChatCommand
    {
        public ObjectLoadCommand() : base("object_load", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("LoadObjectRPC", PhotonTargets.AllBuffered, args[0], args[1]);
            return true;
        }
    }
}