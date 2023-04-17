using Anarchy.Commands.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UIPopupList;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class ExplosionCommand : ChatCommand
    {
        public ExplosionCommand() : base("explosion", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            Vector3 Position = new Vector3(Convert.ToSingle(args[0]), Convert.ToSingle(args[1]), Convert.ToSingle(args[2]));
            Vector3 radius = new Vector3(Convert.ToSingle(args[3]), Convert.ToSingle(args[4]), Convert.ToSingle(args[5]));
            PhotonNetwork.Instantiate("FX/boom1", Position, Quaternion.Euler(270f, 0f, 0f), 0).transform.localScale = radius;
            foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (Vector3.Distance(obj2.transform.position, Position) < radius.y)
                {
                    obj2.GetComponent<HERO>().MarkDie();
                    obj2.GetComponent<HERO>().BasePV.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Explosion " });
                }
            }
            return true;
        }
    }
}
