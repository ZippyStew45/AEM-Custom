using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class ComeHereCommand : ChatCommand
    {
        public ComeHereCommand() : base("comehere", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            Vector3 Mypos = PhotonPlayer.MyHero().transform.position;
            int ID = Convert.ToInt32(args[0]);
            if (ID == PhotonNetwork.player.ID)
            {
                foreach (TITAN titan in FengGameManagerMKII.FGM.titans)
                {
                    if (titan.BasePV.IsMine)
                    {
                        titan.transform.position = Mypos;
                    }
                }
                return true;
            }
            else
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
                {
                    go.GetComponent<HERO>().BasePV.RPC("moveToRPC", PhotonPlayer.Find(ID), new object[] { Mypos.x, Mypos.y, Mypos.z });
                }
                return true;
            }
        }
    }
}