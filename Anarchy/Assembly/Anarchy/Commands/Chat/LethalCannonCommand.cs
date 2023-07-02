using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Commands.Chat
{
    internal class LethalCannonCommand : ChatCommand
    {
        public LethalCannonCommand() : base("lethal", true, false, true)
        {
        }

        public override bool Execute(string[] args)
        {
            bool lethal = false;

            if (args[0].ToLower() == "true") lethal = true;
            else if(args[0].ToLower() == "false") lethal = false;
            else return false;


            if (args[1] == "all")
            {

                FengGameManagerMKII.FGM.BasePV.RPC("LethalCannonBalls", PhotonTargets.AllBuffered, lethal);
                return true;
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("LethalCannonBalls", PhotonPlayer.Find(Convert.ToInt32(args[1])), lethal);
                return true;
            }
        }
    }
}