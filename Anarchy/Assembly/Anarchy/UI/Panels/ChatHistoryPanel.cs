using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Anarchy.UI.GUI;
using static Optimization.Caching.Colors;

namespace Anarchy.UI
{
    public class ChatHistoryPanel : GUIPanel
    {
        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect rect;
        private SmartRect left;
        private SmartRect right;
        private SmartRect scrollRect;
        private GUIStyle style;
        private string showString = string.Empty;
        private readonly List<string> messages = new List<string>();

        public ChatHistoryPanel() : base(nameof(ChatHistoryPanel))
        {
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedRoom, (args) => { messages.Clear(); showString = string.Empty; });
        }

        public void AddMessage(string message)
        {
            messages.Add(message);
            if (IsActive)
            {
                showString += "\n" + message;
            }
        }

        public void ClearMessages()
        {
            messages.Clear();
        }

        protected override void DrawMainPart()
        {
            left.Reset();
            right.Reset();
            rect.Reset();
            rect.MoveY();
            scrollRect.Reset();
            scrollArea.y = rect.y;

            if (messages.Count > 0)
            {
                scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
                UnityEngine.GUILayout.TextArea(showString, Style.Label, new GUILayoutOption[] { UnityEngine.GUILayout.Width(scrollArea.width) });
                //var options = new GUILayoutOption[0];
                //foreach (var msg in messages)
                //{
                //    GUILayout.Label(msg.ToString(), options);
                //}
                EndScrollView();
            }
            if (Button(right, "Back", true))
            {
                Disable();
            }
            if (Button(left, "Save", true))
            {
                File.AppendAllText(Environment.CurrentDirectory + "\\AoTTG_Data\\Logs\\ChatHistory.txt", $"\n-----------------Chat Saved! {DateTime.Now}-----------------\n");
                File.AppendAllText(Environment.CurrentDirectory + "\\AoTTG_Data\\Logs\\ChatHistory.txt", showString.RemoveAll());
                AddMessage($"Chat Saved To: {Environment.CurrentDirectory}\\AoTTG_Data\\Logs\\ChatHistory.txt");
            }
        }

        protected override void OnPanelDisable()
        {
            Screen.lockCursor = Application.loadedLevelName == "menu" ? false : (IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS);
            Screen.showCursor = Application.loadedLevelName == "menu";
            if (!AnarchyManager.Pause.IsActive)
            {
                IN_GAME_MAIN_CAMERA.isPausing = false;
                InputManager.MenuOn = false;
            }
            showString = string.Empty;
        }

        protected override void OnPanelEnable()
        {
            if (!AnarchyManager.Pause.IsActive)
            {
                IN_GAME_MAIN_CAMERA.isPausing = true;
                InputManager.MenuOn = true;
                if (Screen.lockCursor)
                {
                    Screen.lockCursor = false;
                }
                if (!Screen.showCursor)
                {
                    Screen.showCursor = true;
                }
            }
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];

            scroll = Optimization.Caching.Vectors.v2zero;
            scrollRect = new SmartRect(0f, 0f, rect.width, rect.height, 0f, Style.VerticalMargin);
            scrollArea = new Rect(rect.x, rect.y, rect.width, WindowPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollAreaView = new Rect(0f, 0f, rect.width, int.MaxValue);


            SmartRect[] rects = Helper.GetSmartRects(WindowPosition, 2);
            left = rects[0];
            right = rects[1];

            var bld = new StringBuilder();
            foreach(string str in messages)
            {
                bld.AppendLine(str);
            }
            showString = bld.ToString();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
            }
        }
    }
}
