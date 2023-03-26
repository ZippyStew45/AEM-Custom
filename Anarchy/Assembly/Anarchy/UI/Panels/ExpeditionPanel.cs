using System;
using Anarchy.Configuration;
using Anarchy.UI.Animation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.UI;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class ExpeditionPanel : GUIPanel

    {
        private SmartRect left;
        private Rect pagePosition;
        private string[] selections = {"Wagons"};
        private string[] wagonNames = {"No Canopy"};
        private string[] wagonFiles = {"WagonNoMesh"};
        private SmartRect rect;
        private SmartRect right;
        private Vector2 scrollVec = Vector2.zero;
        private SmartRect scrollRect;
        private Rect scrollView;
        private Rect scrollArea;
        private int selectedWagon;

        public ExpeditionPanel() : base(nameof(ExpeditionPanel), GUILayers.SettingsPanel)
        {
        }

        protected override void DrawMainPart()
        {
            rect.Reset();
            float offset = new AutoScaleFloat(120f);
            rect.MoveOffsetX(offset);
            rect.width -= offset;
            pageSelection = SelectionGrid(rect, pageSelection, selections, selections.Length, true);
            SmartRect closeRect = new SmartRect(0, 0, 144, Style.Height);
            closeRect.MoveToEndY(WindowPosition, Style.Height);
            closeRect.MoveToEndX(WindowPosition, 144);
            if (Button(closeRect, "Close"))
            {
                Disable();
            }
        }
        [GUIPage(0)]
        private void WagonsPage()
        {
            right.Reset();
            scrollRect = new SmartRect(0f, 0f, right.width, Style.Height, 0f, Style.VerticalMargin);
            scrollArea = new Rect(right.x, 0f, right.width, Style.Height * 10 + Style.VerticalMargin * (10 - 1));
            scrollArea.y = right.y;
            scrollView = new Rect(0f, 0f, right.width, Style.Height * wagonNames.Length + (Style.VerticalMargin * (wagonNames.Length - 1)));
            scrollRect.Reset();
            left.Reset();
            LabelCenter(right, "Wagons", true);
            scrollVec = BeginScrollView(scrollArea, scrollVec, scrollView);
            {
                for (int i = 0; i < wagonNames.Length; i++)
                {
                    if (Button(scrollRect, wagonNames[i], true))
                    {
                        selectedWagon = i;
                    }
                }
            }
            EndScrollView();
            ToggleButton(left, Settings.WagonRefill, "Refill Station", true);
            if (Button(left, "Spawn", true) && PhotonNetwork.player.Wagoneer)
            {
                IN_GAME_MAIN_CAMERA.MainHERO.CreateWagon(Settings.WagonRefill, wagonFiles[selectedWagon]);
            }
            if (Button(left, "Drop", true))
            {
                IN_GAME_MAIN_CAMERA.MainHERO.DropWagon();
            }

            if (Button(left, "Connect", true))
            {
                IN_GAME_MAIN_CAMERA.MainHERO.TryConnectWagon();
            }
        }

        protected override void OnPanelDisable()
        {
            UIManager.HUDScaleGUI.Value = (float)System.Math.Round(UIManager.HUDScaleGUI.Value, 2);
            head = null;
            left = null;
            rect = null;
            right = null;
            if (Application.loadedLevelName == "menu")
            {
                AnarchyManager.MainMenu.EnableImmediate();
            }
        }
        protected override void OnPanelEnable()
        {
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];
            head = "Expedition";
            pagePosition = new Rect(WindowPosition.x, WindowPosition.y + ((rect.height + Style.VerticalMargin) * 2f), WindowPosition.width, WindowPosition.height - (rect.y + rect.height + Style.VerticalMargin) - Style.WindowBottomOffset - Style.WindowTopOffset);
            SmartRect[] rects = Helper.GetSmartRects(pagePosition, 2);
            left = rects[0];
            right = rects[1];
        }
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
        }
    }
}
