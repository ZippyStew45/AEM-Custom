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
            bool RockThrow = false;
            bool speedie = false;
            bool stalker = false;
            bool faker = false;

            if (args[11] == "1")
            {
                RockThrow = true;
            }
            if (args[12] == "1")
            {
                speedie = true;
            }
            if (args[13] == "1")
            {
                stalker = true;
            }
            if (args[10] == "1")
            {
                faker = true;
            }

            try
            {
                int length = args.Length;
                FengGameManagerMKII.FGM.SPTitan(int.Parse(args[0]), float.Parse(args[1]), int.Parse(args[2]),
                    float.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]),
                    float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]),
                    true, length >= 11 ? faker : false, length >= 12 ? RockThrow : false, length >= 13 ? speedie : false, length >= 14 ? stalker : false, length >= 15 ? args[14] : "", length >= 16 ? args[15] : "", length >= 17 ? float.Parse(args[16]) : 1f);
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