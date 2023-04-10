using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using RC;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SetSkyCommand : ChatCommand
    {
        public SetSkyCommand() : base("setsky", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {

            string[] customSkin = new string[7] { args[0], args[1], args[2], args[3], args[4], args[5], "" };
            if (SkinSettings.CustomMapSet.Value != StringSetting.NotDefine)
            {
                if (SkinSettings.CustomSkins.Value == 1)
                {
                    var set = new CustomMapPreset(SkinSettings.CustomMapSet.Value);
                    set.Load();
                    customSkin = set.ToSkinData();
                }
            }
            object[] argss = new object[] { customSkin, (int)RCManager.GameType.ToValue() };
            FengGameManagerMKII.FGM.BasePV.RPC("clearlevel", PhotonTargets.AllBuffered, argss);
            return true;
        }
    }
}