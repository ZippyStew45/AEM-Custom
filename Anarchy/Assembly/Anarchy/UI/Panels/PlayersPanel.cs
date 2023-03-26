using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Anarchy.Configuration;
using Anarchy.UI.Animation;
using System.Diagnostics;
using System.Linq;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.UI;
using ExitGames.Client.Photon;
using UnityEngine;
using static Anarchy.UI.GUI;
using Object = UnityEngine.Object;

namespace Anarchy.UI
{
    public class PlayersPanel : GUIPanel

    {
        private SmartRect left;
        private SmartRect right;
        private Vector2 playerScroll = Vector2.zero;
        private SmartRect playersRect;
        private Rect playersView;
        private Rect playersArea;
        private PhotonPlayer selectedPlayer;
        private int selectedPlayerACL;
        private int selectedPlayerGAS;
        private int selectedPlayerBLA;
        private int selectedPlayerSPD;
        private string tpCoords = string.Empty;
        private string[] selections = {"Properties", "Stats"};
        
        public PlayersPanel() : base(nameof(PlayersPanel), GUILayers.SettingsPanel)
        {
        }

        [GUIPage(1)]
        private void StatsPage()
        {
            TextField(right, ref selectedPlayerACL, "ACL", Style.LabelOffset, true);
            TextField(right, ref selectedPlayerSPD, "SPD", Style.LabelOffset, true);
            TextField(right, ref selectedPlayerGAS, "GAS", Style.LabelOffset, true);
            TextField(right, ref selectedPlayerBLA, "BLA", Style.LabelOffset, true);
            if (Button(right, "Set", true))
            {
                selectedPlayer.StatOverride = true;
                FengGameManagerMKII.FGM.BasePV.RPC("ForceStatsRPC", selectedPlayer, true, selectedPlayerGAS,
                    selectedPlayerBLA, selectedPlayerSPD, selectedPlayerACL);
            }
        }
        [GUIPage(0)]
        private void PropertiesPage()
        {
            ToggleButton(right, selectedPlayer.RCIgnored, val => { selectedPlayer.RCIgnored = val; }, "Ignored", true);
            ToggleButton(right, selectedPlayer.Wagoneer, val => { selectedPlayer.Wagoneer = val; }, "Wagoneer", true);
            ToggleButton(right, selectedPlayer.Medic, val => { selectedPlayer.Medic = val; }, "Medic", true);
            ToggleButton(right, selectedPlayer.Gunner, val => { selectedPlayer.Gunner = val; }, "Gunner", true);
            ToggleButton(right, selectedPlayer.Supply, val => { selectedPlayer.Supply = val; }, "Supplier", true);
            ToggleButton(right, selectedPlayer.Builder, val => { selectedPlayer.Builder = val; }, "Builder", true);
            if (Button(right, "Kick", true) && !selectedPlayer.IsLocal) Network.Antis.Kick(selectedPlayer, false, string.Empty);
            if (Button(right, "Ban", true) && !selectedPlayer.IsLocal) Network.Antis.Kick(selectedPlayer, true, string.Empty);

            right.MoveY();

            if (Button(right, "TP To Me", true))
            {
                Vector3 Mypos = PhotonPlayer.MyHero().transform.position;
                string[] tpCoordsSplit = tpCoords.Split(' ');
                selectedPlayer.GameObject.GetComponent<HERO>().BasePV.RPC("moveToRPC", selectedPlayer, new object[]
                {
                    Mypos.x, Mypos.y, Mypos.z
                });
            }
            if (Button(right, "TP To Player", true))
            {
                GameObject gameObject1 = new GameObject();
                GameObject gameObject2 = new GameObject();
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (go.GetPhotonView().owner == PhotonPlayer.Find(selectedPlayer.ID))
                        gameObject1 = go;
                    if (go.GetPhotonView().owner == PhotonNetwork.player)
                        gameObject2 = go;
                }
                gameObject2.transform.position = gameObject1.transform.position;
            }
            if (Button(right, "TP All To Me", true))
            {
                Vector3 Mypos = PhotonPlayer.MyHero().transform.position;
                selectedPlayer.GameObject.GetComponent<HERO>().BasePV.RPC("moveToRPC", PhotonTargets.All, new object[]
                {
                    Mypos.x, Mypos.y, Mypos.z
                });
            }
            if (Button(right, "TP To Coords", true))
            {
                string[] tpCoordsSplit = tpCoords.Split(' ');
                selectedPlayer.GameObject.GetComponent<HERO>().BasePV.RPC("moveToRPC", selectedPlayer, new object[]
                {
                    float.Parse(tpCoordsSplit[0]),
                    float.Parse(tpCoordsSplit[1]),
                    float.Parse(tpCoordsSplit[2])
                });
            }
            tpCoords = TextField(right, tpCoords, "Coords", Style.LabelOffset, true);
        }

        protected override void DrawMainPart()
        {
            //Player selector
            left.Reset();
            LabelCenter(left, "Select Player");
            left.MoveY();
            playersArea = new Rect(left.x, 0f, left.width, Style.Height * 10 + Style.VerticalMargin * (10 - 1));
            playersView = new Rect(0f, 0f, left.width, Style.Height * PhotonNetwork.playerList.Length + (Style.VerticalMargin * (PhotonNetwork.playerList.Length - 1)));
            playersArea.y = left.y;
            playersRect.Reset();
            playerScroll = BeginScrollView(playersArea, playerScroll, playersView);
            {
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    if (Button(playersRect, PhotonNetwork.playerList[i].UIName.ToHTMLFormat() + " [" + PhotonNetwork.playerList[i].ID + "]", true))
                    {
                        selectedPlayer = PhotonNetwork.playerList[i];
                        UpdateStats();
                    }
                }
            }
            EndScrollView();
            right.Reset();
            LabelCenter(right, selectedPlayer.UIName.ToHTMLFormat(), true);
            pageSelection = SelectionGrid(right, pageSelection, selections, selections.Length, true);
            right.MoveY();
            SmartRect rect = new SmartRect(0, 0, 144, Style.Height);
            rect.MoveToEndY(WindowPosition, Style.Height);
            rect.MoveToEndX(WindowPosition, 144);
            if (Button(rect, "Close"))
            {
                Disable();
            }
        }

        protected override void OnPanelDisable()
        {
            UIManager.HUDScaleGUI.Value = (float)System.Math.Round(UIManager.HUDScaleGUI.Value, 2);
            head = null;
            left = null;
            right = null;
            if (Application.loadedLevelName == "menu")
            {
                AnarchyManager.MainMenu.EnableImmediate();
            }
        }

        private void UpdateStats()
        {
            try
            {
                selectedPlayerACL = (int) selectedPlayer.AllProperties[PhotonPlayerProperty.statACL];
                selectedPlayerSPD = (int) selectedPlayer.AllProperties[PhotonPlayerProperty.statSPD];
                selectedPlayerBLA = (int) selectedPlayer.AllProperties[PhotonPlayerProperty.statBLA];
                selectedPlayerGAS = (int) selectedPlayer.AllProperties[PhotonPlayerProperty.statGAS];
            }
            catch
            {
                selectedPlayerACL = 0;
                selectedPlayerSPD = 0;
                selectedPlayerBLA = 0;
                selectedPlayerGAS = 0;
            }
        }
        protected override void OnPanelEnable()
        {
            selectedPlayer = PhotonNetwork.playerList[0];
            UpdateStats();
            head = "Players";
            SmartRect[] rects = Helper.GetSmartRects(WindowPosition, 2);
            left = rects[0];
            right = rects[1];
            playersRect = new SmartRect(0f, 0f, left.width, Style.Height, 0f, Style.VerticalMargin);
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
