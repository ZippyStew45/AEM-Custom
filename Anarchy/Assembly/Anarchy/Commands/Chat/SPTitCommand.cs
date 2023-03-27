using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using Anarchy.UI;
using Anarchy.UI.Animation;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SPTitCommand : ChatCommand
    {

        public SPTitCommand() : base("sptit", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            bool fake = false;
            if (args[10] == "1")
            {
                fake = true;
                int ran = UnityEngine.Random.Range(0, 3);
                string[] animation = { "run_walk", "run_abnormal", "run_abnormal_1" };
                switch (ran)
                {
                    case 0:
                        TITAN.runAnimation2 = animation[0];
                        break;
                    case 1:
                        TITAN.runAnimation2 = animation[1];
                        break;
                    case 2:
                        TITAN.runAnimation2 = animation[2];
                        break;
                    default:
                        TITAN.runAnimation2 = animation[0];
                        break;
                }
            }
            else TITAN.runAnimation2 = null;
            try
            {
                int length = args.Length;
                FengGameManagerMKII.FGM.SPTitan(int.Parse(args[0]), float.Parse(args[1]), int.Parse(args[2]),
                    float.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]),
                    float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]),
                    true, fake, length >= 12 ? args[11] : "", length >= 13 ? args[12] : "", length >= 14 ? float.Parse(args[13]) : 1f);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
        }
    }
}