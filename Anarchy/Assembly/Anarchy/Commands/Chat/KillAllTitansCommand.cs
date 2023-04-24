using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class KillAllTitansCommand : ChatCommand
    {
        public KillAllTitansCommand() : base("killtitans", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("titan"))
            {
                if (!gameObject.GetComponent<TITAN>().hasDie)
                {
                    TITAN component = gameObject.GetComponent<TITAN>();
                    component.BasePV.RPC("netDie", PhotonTargets.All, new object[0]);
                }
            }
            string content = "<color=00FF00>Titans Are Getting Killed!</color>";
            FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, new object[] { content, string.Empty });
            return true;
        }
    }
}
