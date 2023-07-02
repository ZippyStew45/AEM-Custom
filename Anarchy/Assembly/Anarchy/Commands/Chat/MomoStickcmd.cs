using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Optimization.Caching;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    internal class MomoStickcmd : ChatCommand
    {
        public MomoStickcmd() : base("stick", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (FengGameManagerMKII.Connor == true)
            {
                FengGameManagerMKII.Link1.Clear();
                FengGameManagerMKII.Connor = false;
                chatMessage = "Not Following player";
                return true;
            }
            else
            {
                foreach (PhotonPlayer players in PhotonNetwork.playerList)
                {
                    if (Convert.ToInt32(args[0]) == players.ID)
                    {
                        FengGameManagerMKII.Link1.Add((object)players.ID, (object)(string)players.Properties[(object)PhotonPlayerProperty.guildName]);
                        FengGameManagerMKII.Link.Clear();
                        FengGameManagerMKII.Connor = true;
                        chatMessage = "Following player";
                        return true;
                    }
                }
            }
            return true;
        }
    }
}
