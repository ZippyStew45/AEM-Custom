﻿using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UGUI = UnityEngine.GUILayout;

namespace Anarchy.UI
{
    internal class Chat : GUIBase
    {
        public const string ChatRPC = "Chat";
        public const string ChatPMRPC = "ChatPM";

        internal static FloatSetting BackgroundTransparency = new FloatSetting("ChatBackgroundTransparency", 0.15f);
        internal static IntSetting MessageCount = new IntSetting("ChatMessageCount", 10);
        internal static BoolSetting UseBackground = new BoolSetting("UseChatBackround", false);
        internal static IntSetting FontSize = new IntSetting("ChatFontSize", 15);
        internal static IntSetting ChatWidth = new IntSetting("ChatWidth", 330);
        internal static BoolSetting UseCustomChatSpace = new BoolSetting("UseCustomChatSpace", false);
        internal static IntSetting CustomChatSpaceUp = new IntSetting("CustomChatSpaceUp", 0);
        internal static IntSetting CustomChatSpaceDown = new IntSetting("CustomChatSpaceDown", 0);
        internal static IntSetting CustomChatSpaceLeft = new IntSetting("CustomChatSpaceLeft", 0);
        internal static IntSetting CustomChatSpaceRight = new IntSetting("CustomChatSpaceRight", 0);

        internal static Chat Instance;

        private Rect position;
        private Rect inputPosition;
        private Vector2 scrollvector;
        private Rect scrollView;
        private Rect scrollAreaView;

        private GUIStyle ChatStyle;
        private GUIStyle textFieldStyle;

        private GUILayoutOption[] textFieldOptions;
        private GUILayoutOption labelOptions;

        private List<string> messages;
        private string inputLine = string.Empty;
        private Vector2 commandsScroll = new Vector2(0, myYScroll);
        static int myYScroll = 0;

        //command list
        public static bool isShowCommandList = false;
        public Rect rectCommands = new Rect(0, 0, 0, 0);

        public static Commands.Chat.ChatCommandHandler CMDHandler = new Commands.Chat.ChatCommandHandler();

        internal Chat() : base("Chat", GUILayers.Chat)
        {
            Instance = this;
            messages = new List<string>();
        }
        public static string GetLastMessages()
        {
            int i = Instance.messages.Count - 11;
            int end = Instance.messages.Count - 1;
            var bld = new System.Text.StringBuilder();
            for (; i < end; i++)
            {
                bld.AppendLine(Instance.messages[i].RemoveHTML());
            }
            return bld.ToString();
        }

        internal static void Add(string message)
        {
            if (Instance == null)
            {
                return;
            }
            FengGameManagerMKII.FGM.StartCoroutine(AddI(message));
        }

        private static System.Collections.IEnumerator AddI(string message)
        {
            yield return new WaitForEndOfFrame();
            List<string> messages = Instance.messages;
            if (message == null)
            {
                yield break;
            }
            if (messages.Count > MessageCount.Value)
            {
                messages.Remove(messages.First());
            }
            messages.Add(message);
            AnarchyManager.ChatHistory.AddMessage(message);
        }

        public static void Clear()
        {
            FengGameManagerMKII.FGM.StartCoroutine(ClearI());
        }

        private static System.Collections.IEnumerator ClearI()
        {
            yield return new WaitForEndOfFrame();
            if (Instance != null && Instance.messages != null)
            {
                Instance.messages.Clear();
            }
        }

        private void ResetInputline()
        {
            inputLine = string.Empty;
            UnityEngine.GUI.FocusControl(string.Empty);
        }

        private static string CheckForMention(string message)
        {
            string sentData = message;
            if (message.Contains("@"))
            {
                var strings = message.Split(' ');
                sentData = string.Empty;
                for (int i = 0; i < strings.Length; i++)
                {
                    string str = strings[i];
                    if (str.Contains("@") && str.Length > 1)
                    {
                        string id = str.Remove(str.IndexOf("@"), 1);
                        if (int.TryParse(id, out int ID))
                        {
                            sentData += (i == 0 ? "" : " ") + "<b>[" + id + "] " + PhotonPlayer.Find(System.Convert.ToInt32(ID)).UIName.ToHTMLFormat() + "</b>";
                        }
                        else sentData += (i == 0 ? "" : " ") + strings[i];
                    }
                    else sentData += (i == 0 ? "" : " ") + str;
                }
            }
            return sentData;
        }

        public static void Send(string message)
        {
            message = CheckForMention(message);
            FengGameManagerMKII.FGM.BasePV.RPC(ChatRPC, PhotonTargets.All, new object[] { User.ChatSend(message), "" });
        }

        protected override void OnEnable()
        {
            position = new Rect(0f, Style.ScreenHeight - 500, ChatWidth * (Style.WindowWidth / 700f), 470f);
            inputPosition = new Rect(30f, Style.ScreenHeight - 300 + 275, 300f * Style.WindowWidth / 700f + (30f * (Style.WindowWidth / 700f) - 30f), 25f);
            ChatStyle = new GUIStyle(Style.Label);
            ChatStyle.normal.textColor = Optimization.Caching.Colors.white;
            textFieldStyle = new GUIStyle(Style.TextField);
            textFieldStyle.fontSize = FontSize;
            if (UseBackground)
            {
                Texture2D black = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                black.SetPixel(0, 0, new Color(0f, 0f, 0f, BackgroundTransparency.Value));
                black.Apply();
                ChatStyle.normal.background = black;
            }
            ChatStyle.fontSize = FontSize;
            if (UseCustomChatSpace.Value)
            {
                ChatStyle.padding = new RectOffset(CustomChatSpaceLeft, CustomChatSpaceRight, CustomChatSpaceUp, CustomChatSpaceDown);
            }
            else
            {
                ChatStyle.padding = new RectOffset(0, 0, 0, 0);
            }
            ChatStyle.border = new RectOffset(0, 0, 0, 0);
            ChatStyle.margin = new RectOffset(0, 0, 0, 0);
            ChatStyle.overflow = new RectOffset(0, 0, 0, 0);
            //ChatStyle.fixedWidth = 330f * (Style.WindowWidth / 700f);
            labelOptions = UnityEngine.GUILayout.MinWidth(position.width);
            textFieldOptions = new GUILayoutOption[] { UnityEngine.GUILayout.Width(inputPosition.width) };
            scrollView = new Rect(0f, 0f, position.width, position.height);
            scrollAreaView = new Rect(0f, 0f, position.width, 2000f);
            scrollvector = Optimization.Caching.Vectors.v2zero;

            this.rectCommands = new Rect(350f, (float)Screen.height - 270f, 384f, 200f);
        }

        protected internal override void Draw()
        {
            Event ev = Event.current;
            if (ev.type == EventType.KeyDown && (ev.keyCode == KeyCode.Tab || ev.character == '\t'))
            {
                return;
            }
            if (ev.type == EventType.KeyDown && ev.keyCode != KeyCode.None && (ev.keyCode == KeyCode.Return || ev.keyCode == KeyCode.KeypadEnter || (InputManager.OpenChatCode != KeyCode.None && ev.keyCode == InputManager.OpenChatCode)))
            {
                if (!inputLine.IsNullOrWhiteSpace())
                {
                    if (inputLine == "\t")
                    {
                        ResetInputline();
                        return;
                    }
                    if (RC.RCManager.RCEvents.ContainsKey("OnChatInput"))
                    {
                        string key = (string)RC.RCManager.RCVariableNames["OnChatInput"];
                        var collection = RC.RCManager.stringVariables;
                        if (collection.ContainsKey(key))
                        {
                            collection[key] = inputLine;
                        }
                        else
                        {
                            collection.Add(key, inputLine);
                        }
                        ((RCEvent)RC.RCManager.RCEvents["OnChatInput"]).checkEvent();
                    }
                    if (inputLine.StartsWith("/"))
                    {
                        CMDHandler.TryHandle(AnarchyExtensions.CommandFormat(inputLine));
                    }
                    else
                    {
                        Send(inputLine);
                    }
                    ResetInputline();
                    return;
                }
                else
                {
                    inputLine = "\t";
                    UnityEngine.GUI.FocusControl("ChatInput");
                }
                return;
            }
            UnityEngine.GUI.SetNextControlName(string.Empty);
            GUILayout.BeginArea(position);
            scrollvector = GUI.BeginScrollView(scrollView, scrollvector, scrollAreaView);
            scrollvector.y = float.PositiveInfinity;
            GUILayout.BeginArea(scrollAreaView);
            UnityEngine.GUILayout.FlexibleSpace();
            for (int i = 0; i < messages.Count; i++)
            {
                string currentMessage = messages[i];
                if (!currentMessage.IsNullOrEmpty())
                {
                    UnityEngine.GUILayout.Label(currentMessage, ChatStyle, labelOptions);
                }
            }
            GUILayout.EndArea();
            GUI.EndScrollView();
            GUILayout.EndArea();
            UnityEngine.GUI.SetNextControlName("ChatInput");
            GUILayout.BeginArea(inputPosition);
            inputLine = UnityEngine.GUILayout.TextField(inputLine, textFieldStyle, textFieldOptions);
            GUILayout.EndArea();

            if (isShowCommandList)
            {
                //Screen.lockCursor = false;
                GUILayout.BeginArea(this.rectCommands);
                UGUI.FlexibleSpace();
                GUILayout.Label(string.Concat(new string[] { "<color=#000000>[Available commands]</color>" }), new GUILayoutOption[0]);
                string text1 = string.Empty;

                this.commandsScroll = GUILayout.BeginScrollView(this.commandsScroll);
                string[] commandlist = this.inputLine.Split(' ');
                foreach (var element in CommandList) //dictionary
                {
                    if (element.Key.ToLower().Contains(commandlist[0]))
                    {
                        GUILayout.Label(element.Key + element.Value, new GUILayoutOption[0]);
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }

            if (isShowCommandList && Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                commandsScroll = new Vector2(0, myYScroll -= 10);
            }
            else if (isShowCommandList && Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                commandsScroll = new Vector2(0, myYScroll += 10);
            }

            if (this.inputLine.StartsWith("/")) isShowCommandList = true;
            else isShowCommandList = false;
        }

        private static Dictionary<string, string> CommandList = new Dictionary<string, string>
        {
        { "/pm ID Message ", "Send A Private Message To A Player" },
        { "/kick ID ", "Kick A Player From The Room" },
        { "/ban ID ", "Ban A Player From The Room" },
        { "/skick ID ", "Super Kick A Player From The Room" },
        { "/sban ID ", "Super Ban A Player From The Room" },
        { "/room hide|show ", "Make Room Invisable/Visable To Lobby" },
        { "/room close|open ", "Make The Room Joinable Or UnJoinable" },
        { "/room max Amount ", "Set The Maximum Amount Of Joinable Players" },
        { "/room time Amount ", "Set The Time Limit Of The Room" },
        { "/pause ", "Pause Everything In Your Room" },
        { "/unpause ", "UnPauses The Room" },
        { "/resetkd IDs|all ", "Reset Sletected Player's K/D Or Reset All K/D's" },
        { "/revive ID ", "Revive A Player" },
        { "/leave ", "Leave Your Current Room" },
        { "/asoracing ", "Sets The Rules To ASO Racing" },
        { "/rules ", "Shows The Current Room's Play Rules" },
        { "/clear ", "Clears Chat" },
        { "/kill ID ", "Kill A Player" },
        { "/team [0|1|2] ", "Set Your Player Team" },
        { "/unban ID ", "Unban A Player" },
        { "/mute ID ", "Mute A Player" },
        { "/unmute ID ", "Unmute A Player" },
        { "/animate ", "Work In Progress" },
        { "/checkuser ID ", "Check Anarchy Abuse Features Of Player" },
        { "/scatter ", "Spread The Titans Around The Map" },
        { "/givegas ID Amount ", "Give A Player A Selected Amount Of Gas" },
        { "/sptit type size health speed count chaseDistance attackWait X Y Z [Optional ->] Faker[0|1] RockThrow[0|1] Speedie[0|1] Stalker[0|1] bodySkinLink eyeSkinLink animationSpeed ", "Spawn Custom Titan (1 = Enable)" },
        { "/emcustommap localPath prefabName X Y Z X Y Z W ", "Spawn Custom Unity Map" },
        { "/bundle_load bundleName ", "Load Unity Bundle To Cache" },
        { "/object_load bundleName objectName ", "Load Unity Object To Cache" },
        { "/object_spawn bundleName objectName X Y Z X Y Z W ", "Spawn Unity Object" },
        { "/moveobject name X Y Z X Y Z W ", "Move An Already Loaded Unity Object" },
        { "/destroyobjects ObjectList ", "Destroy Objects Listed" },
        { "/fog start Distance ", "Set Distance For Fog To Start" },
        { "/fog end Distance ", "Set Distance For Fog To End" },
        { "/fog density Value ", "Set Fog Density" },
        { "/fog color HEX ", "Set Fog Color" },
        { "/fog enabled Value ", "Turn On Fog" },
        { "/fog mode ExponentialSquared|Exponential|Linear ", "Set Fog Mode" },
        { "/commonvd Amount ", "Set The Titans Detection Radius" },
        { "/seths ID Amount ", "Set A Player's Horse Speed" },
        { "/light Color Hex ", "Set Light Color" },
        { "/light Intensity Amount ", "Set Light Levels" },
        { "/gravity Amount ", "Set The Gravity Level" },
        { "/impact True|False Speed ", "Enable/Disable Player Collision Damage" },
        { "/fx Object X Y Z X Y Z W ", "Spawn Effect" },
        { "/rd Amount ", "Set Your Local Render Distance" },
        { "/obj Object ", "Spawn Primitive object, default objects are: cube, Sphere, Capsule, Cylinder, Quad, Plane (You Need Builder Role To Use This)" },
        { "/cannon ", "Spawn a Cannon (You Need Gunner Role To Use This)" },
        { "/givelight ID Toggle(0|1) ", "Set Light On A Player Or Turn It Off" },
        { "/setdaylightcolor R [Optional]G [Optional]B ", "Set Day Light Level (ex: 0 is MidNight, 1 is blinding)" },
        { "/setsky north south east west top bottom ", "Set SkyBox For Everyone" },
        { "/comehereall ", "TP all PLayers To You" },
        { "/comehere ID ", "TP Certain PLayers To You, If You Use Your ID It TPs All Titans On You" },
        { "/killtitans ", "Kills All Titans In The Room" },
        { "/difficulty [0|1|2] ", "Change Difficulty Of All AEM Players (0 = normal, 1 = hard, 2 = abnormal)" },
        { "/cmdlist File ", " Pick A File To Run Via Command List" },
        { "/explosion Position(X Y Z) Size(X Y Z) ", " Spawn An Explosion That Kills Players (Y Size Determines Death Radius)" },
        { "/r ", "Restart The Current Lobby" },
        { "/sup ", "Spawn Supply Station (Requires Wagon Role)" },
        { "/notif Duration Message ", "Sends a notification to all AEM players" },
        { "/shakescreen Intensity Duration Decay ", "Shake Player's Screens for dramatic effect" },
        { "/lethal true/false ID/all ", "Shake Player's Screens for dramatic effect" },
        { "/restart ", "Restart The Current Lobby" } };

        protected override void OnDisable()
        {
        }

        public static void SendLocalizedText(PhotonPlayer target, string file, string key, string[] args)
        {
            string[] sendArgs = args ?? new string[0];
            if (!target.AnarchySync)
            {
                Localization.Locale loc;
                bool needClose = false;
                if (Localization.Language.SelectedLanguage != "English")
                {
                    loc = new Localization.Locale("English", file, true, ',');
                    loc.Load();
                    needClose = true;
                }
                else
                {
                    loc = Localization.Language.Find(file) ?? new Localization.Locale(file, true, ',');
                    if (!loc.IsOpen)
                    {
                        loc.Load();
                        needClose = true;
                    }
                }
                string content = sendArgs.Length <= 0 ? loc[key] : loc.Format(key, sendArgs);
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", target, new object[] { content, string.Empty });
                if (needClose)
                {
                    loc.Unload();
                }
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("ChatLocalized", target, new object[] { file, key, sendArgs });
            }
        }

        public static void SendLocalizedText(string file, string key, string[] args)
        {
            string[] sendArgs = args ?? new string[0]; if (Localization.Language.SelectedLanguage != Localization.Language.DefaultLanguage)
            {
                Localization.Locale loc = new Localization.Locale("English", file, true, ',');
                loc.Load();
                string content = sendArgs.Length <= 0 ? loc[key] : loc.Format(key, sendArgs);
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.NotAnarchy, new object[] { content, string.Empty });
                loc.Unload();
            }
            else
            {
                Localization.Locale loc = Localization.Language.Find(file);
                bool needClose = false;
                if (!loc.IsOpen)
                {
                    loc.Load();
                    needClose = true;
                }
                string content = sendArgs.Length <= 0 ? loc[key] : loc.Format(key, sendArgs);
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.NotAnarchy, new object[] { content, string.Empty });
                if (needClose)
                {
                    loc.Unload();
                }
            }
            FengGameManagerMKII.FGM.BasePV.RPC("ChatLocalized", PhotonTargets.AnarchyUsersOthers, new object[] { file, key, sendArgs });
        }

        public static void SendLocalizedTextAll(string file, string key, string[] args)
        {
            string[] sendArgs = args ?? new string[0]; if (Localization.Language.SelectedLanguage != Localization.Language.DefaultLanguage)
            {
                Localization.Locale loc = new Localization.Locale("English", file, true, ',');
                loc.Load();
                string content = sendArgs.Length <= 0 ? loc[key] : loc.Format(key, sendArgs);
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.NotAnarchy, new object[] { content, string.Empty });
                loc.Unload();
            }
            else
            {
                Localization.Locale loc = Localization.Language.Find(file);
                bool needClose = false;
                if (!loc.IsOpen)
                {
                    loc.Load();
                    needClose = true;
                }
                string content = sendArgs.Length <= 0 ? loc[key] : loc.Format(key, sendArgs);
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.NotAnarchy, new object[] { content, string.Empty });
                if (needClose)
                {
                    loc.Unload();
                }
            }
            FengGameManagerMKII.FGM.BasePV.RPC("ChatLocalized", PhotonTargets.AnarchyUsers, new object[] { file, key, sendArgs });
        }
    }
}