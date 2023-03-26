using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
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
            try
            {
                int length = args.Length;
                FengGameManagerMKII.FGM.SPTitan(int.Parse(args[0]), float.Parse(args[1]), int.Parse(args[2]),
                    float.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]),
                    float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]),
                    Convert.ToBoolean(int.Parse(args[10])), length >= 12 ? args[11] : "", length >= 13 ? args[12] : "", length >= 14 ? float.Parse(args[13]) : 1f);
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