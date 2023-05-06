using Anarchy.Commands.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Mono.Security.X509.X520;

namespace AoTTG.Anarchy.Commands.Chat
{

    internal class NotifCommand : ChatCommand
    {
        private int pmID = -1;

        public NotifCommand() : base("notif", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            string message = string.Join(" ", args, 1, args.Length - 1);
            FengGameManagerMKII.FGM.BasePV.RPC("NotifRPC", PhotonTargets.All, new object[] { message, float.Parse(args[0]) });
            return true;
        }
    }
}