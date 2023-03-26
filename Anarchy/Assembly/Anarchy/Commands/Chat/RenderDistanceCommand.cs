using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using UnityEngine;
using Anarchy;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class RenderDistanceCommand : ChatCommand
    {
        public RenderDistanceCommand() : base("rd", false, true, true)
        {

        }

        public override bool Execute(string[] args)
        {
            float ID = float.Parse(args[0]);
            Camera.main.farClipPlane = ID;
            chatMessage = "Render Distance set to: " + args[0];
            return true;
        }
    }
}