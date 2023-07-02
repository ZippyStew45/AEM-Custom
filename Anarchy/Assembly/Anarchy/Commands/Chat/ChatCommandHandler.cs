using System.Collections.Generic;
using AoTTG.Anarchy.Commands.Chat;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    /// <summary>
    /// Handler for <seealso cref="UI.Chat"/> commands
    /// </summary>
    internal class ChatCommandHandler
    {
        private Dictionary<string, ChatCommand> allCommands;

        public ChatCommandHandler()
        {
            allCommands = new Dictionary<string, ChatCommand>();
            ChatCommand.LoadLocale();
            InitCommands();
        }

        private void InitCommands()
        {
            allCommands.Clear();
            allCommands.Add("restart", new RestartCommand());
            allCommands.Add("r", new RestartCommand());
            allCommands.Add("pm", new PMCommand());
            allCommands.Add("kick", new KickCommand(false, false));
            allCommands.Add("ban", new KickCommand(true, false));
            allCommands.Add("skick", new KickCommand(false, true));
            allCommands.Add("sban", new KickCommand(true, true));
            allCommands.Add("room", new RoomSettingCommands());
            allCommands.Add("pause", new PauseCommand(true));
            allCommands.Add("unpause", new PauseCommand(false));
            allCommands.Add("resetkd", new ResetKDCommand());
            allCommands.Add("revive", new ReviveCommand());
            allCommands.Add("fx", new SpawnFX());
            //Temporarily removed from mod
            //allCommands.Add("spectate", new SpectateCommand());
            allCommands.Add("leave", new LeaveCommand());
            allCommands.Add("asoracing", new ASORacingCommand());
            allCommands.Add("rules", new RulesCommand());
            allCommands.Add("clear", new ClearCommand());
            allCommands.Add("kill", new KillCommand());
            allCommands.Add("team", new ChangeTeamCommand());
            allCommands.Add("unban", new UnbanCommand());
            allCommands.Add("mute", new MuteCommand(true));
            allCommands.Add("unmute", new MuteCommand(false));
            allCommands.Add("animate", new AnimateNameCommand());
            allCommands.Add("checkuser", new CheckAnarchyUserCommand());
            allCommands.Add("scatter", new ScatterCommand());
            allCommands.Add("givegas", new GiveGasCommand());
            allCommands.Add("test", new TestCommand());
            allCommands.Add("sptit", new SPTitCommand());
            allCommands.Add("emcustommap", new EMCustomMapCommand());
            allCommands.Add("cannon", new SpawnCannonCommand());
            allCommands.Add("bundle_load", new BundleLoadCommand());
            allCommands.Add("object_load", new ObjectLoadCommand());
            allCommands.Add("object_spawn", new ObjectSpawnCommand());
            allCommands.Add("commonvd", new CommonVDCommand());
            allCommands.Add("seths", new SetHSCommand());
            allCommands.Add("fog", new FogCommand());
            allCommands.Add("light", new LightCommand());
            allCommands.Add("givelight", new SetFlashLightCommand());
            allCommands.Add("setsky", new SetSkyCommand());
            allCommands.Add("setdaylightcolor", new SetDayLevelCommand());
            allCommands.Add("gravity", new GravityCommand());
            allCommands.Add("obj", new SpawnPrimitiveCommand());
            allCommands.Add("impact", new ImpactDeathCommand());
            allCommands.Add("moveobject", new MoveObjectCommand());
            allCommands.Add("destroyobjects", new DestroyObjectsCommand());
            allCommands.Add("rd", new RenderDistanceCommand());
            allCommands.Add("comehereall", new ComeHereAllCommand());
            allCommands.Add("comehere", new ComeHereCommand());
            allCommands.Add("killtitans", new KillAllTitansCommand());
            allCommands.Add("difficulty", new ChangeDifficultyCommand());
            allCommands.Add("cmdlist", new RunCMDListCommand());
            allCommands.Add("explosion", new ExplosionCommand());
            allCommands.Add("shakescreen", new ShakeScreenCommand());
            allCommands.Add("notif", new NotifCommand());
            allCommands.Add("sup", new SpawnSupplyStationCommand());
            allCommands.Add("stick", new MomoStickcmd());
        }

        private void NotFound(string name)
        {
            string message = User.FormatColors(ChatCommand.Lang.Format("cmdNotFound", name));
            if (AnarchyManager.Log.IsActive)
            {
                UI.Log.AddLineRaw(message, UI.MsgType.Error);
            }
            else
            {
                UI.Chat.Add(message);
            }
        }

        public void TryHandleCommand(ICommand cmd, string inputLine)
        {
            if (cmd.Execute(inputLine.Split(' ')))
            {
                cmd.OnSuccess();
            }
            else
            {
                cmd.OnFail();
            }
        }

        /// <summary>
        /// Tries to handle command from given string
        /// </summary>
        /// <param name="inputLine">Chat line</param>
        public void TryHandle(string inputLine)
        {
            string[] strArray = inputLine.Substring(1).Split(' ');
            if (!allCommands.TryGetValue(strArray[0].ToLower(), out ChatCommand cmd))
            {
                NotFound(strArray[0].ToLower());
                return;
            }
            if (cmd.RequireMC && !PhotonNetwork.IsMasterClient)
            {
                UI.Chat.Add(ChatCommand.Lang["errMC"]);
                return;
            }
            string[] args = CommandsExtensions.ParseCommandArgs(inputLine, 1);
            try
            {
                if (cmd.Execute(args))
                {
                    cmd.OnSuccess();
                }
                else
                {
                    cmd.OnFail();
                }
            }
            catch (System.Exception ex)
            {
                UI.Chat.Add(ChatCommand.Lang.Format("errExecute", cmd.CommandName));
                UnityEngine.Debug.Log($"Exception occured while executing command: {cmd.CommandName}\nException message: {ex.Message}\nStackTrace:\n{ex.StackTrace}");
            }
            cmd.OnFinalize();
        }
    }
}
