using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using ExitGames.Client.Photon;
using Anarchy.Commands.Chat;
using System.Threading;
using Anarchy;

namespace Mod
{
    internal class CommandList : MonoBehaviour
    {
        public static CommandList CMD;
        public static bool IsCrashlistVisible;
        Rect crashlistrect = new Rect(25f, 25f, 380f, 500f);
        public static Vector2 crashscroll = Vector2.zero;
        string AddOnPlayerBanlist;
        int to_read_banlist;
        static string CommandListPath;
        public static System.Collections.Generic.List<string> banlist;
        bool doneapply;
        bool doneedit;

        public static Anarchy.Commands.Chat.ChatCommandHandler CMDHandler = new Anarchy.Commands.Chat.ChatCommandHandler();


        private void Update()
        {
            if (EMInputManager.IsInputDown(EMInputManager.EMInputs.CommandList))
            {
                IsCrashlistVisible = !IsCrashlistVisible;
                if (IsCrashlistVisible == true)
                {
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                }
                else if (IsCrashlistVisible == false && PhotonNetwork.inRoom)
                {
                    Screen.showCursor = false;
                    Screen.lockCursor = true;
                }
                else if (IsCrashlistVisible == false && !PhotonNetwork.inRoom)
                {
                    Screen.showCursor = true;
                    Screen.lockCursor = false;
                }
                //else if (IsCrashlistVisible == false && NameAnimation.IsNAnimationVisible != true && DetectFaggot.DetectIsVisible != true) Screen.lockCursor = true; //locks it at center
            }
        }

        private void OnGUI()
        {
            if (IsCrashlistVisible)
            {
                crashlistrect = UnityEngine.GUI.Window(640, this.crashlistrect, new UnityEngine.GUI.WindowFunction(this.CommandListGUI), string.Empty);
            }
        }

        private void Start()
        {
            this.AddOnPlayerBanlist = string.Empty;
            CommandListPath = Environment.CurrentDirectory + "\\AoTTG_Data\\CommandLists\\";
            this.Load_Commands();
        }

        private void Load_Commands()
        {
            banlist = new System.Collections.Generic.List<string>();
            string[] bbas = Directory.GetFiles(CommandListPath, "*.txt");
            foreach (string leans in bbas) banlist.Add(leans);
            return;
        }


        private CommandList()
        {
            this.to_read_banlist = -1;
        }

        private void CommandListGUI(int ID)
        {
            UnityEngine.GUILayout.BeginHorizontal();

            if (UnityEngine.GUILayout.Button("Clear", new GUILayoutOption[0]))
            {
                string[] bbas = Directory.GetFiles(CommandListPath, "*.txt");
                for (int i = 0; i < banlist.Count; i++)
                {
                    File.Delete(bbas[i]);
                }
                banlist.Clear();
            }
            else if (UnityEngine.GUILayout.Button("Scroll Down", new GUILayoutOption[0]))
            {
                crashscroll.y = float.PositiveInfinity;
            }
            else if (UnityEngine.GUILayout.Button("Refresh", new GUILayoutOption[0]))
            {
                this.Load_Commands();
            }
            UnityEngine.GUILayout.EndHorizontal();
            crashscroll = UnityEngine.GUILayout.BeginScrollView(crashscroll, new GUILayoutOption[0]);
            if (banlist.Count == 0)
            {
                UnityEngine.GUILayout.Label("<color=#FF0000>No Files In Folder</color>", new GUILayoutOption[0]);
            }
            else
            {
                for (int i = 0; i < banlist.Count; i++)
                {
                    string nameplayer4 = banlist[i];
                    UnityEngine.GUILayout.BeginHorizontal(UnityEngine.GUI.skin.box, new GUILayoutOption[0]);

                    if (UnityEngine.GUILayout.Button("x", new GUILayoutOption[] { UnityEngine.GUILayout.Width(40f) }))
                    {
                        banlist.Remove(nameplayer4);
                        this.to_read_banlist = -1;
                        File.Delete(nameplayer4);
                    }

                    string[] options = banlist[i].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    string filename = options.Last().Replace(".txt","");
                    if (UnityEngine.GUILayout.Button(filename, new GUILayoutOption[] { UnityEngine.GUILayout.Width(280f) }))
                    {
                        //Run File In Enum for later when all cmds are added
                        FengGameManagerMKII.FGM.StartCoroutine(RunCmdtest(banlist[i]));
                    }
                    UnityEngine.GUILayout.EndHorizontal();
                }
            }
            UnityEngine.GUILayout.EndScrollView();
            UnityEngine.GUI.DragWindow();
        }

        public System.Collections.IEnumerator RunCmdtest(string FilePath)
        {
            string[] linesRead = File.ReadAllLines(FilePath);
            Vector3 ppos = PhotonPlayer.MyHero().transform.position;
            Quaternion prot = PhotonPlayer.MyHero().transform.rotation;
            foreach (string line in linesRead)
            {
                line.ToLower();
                string[] options = line.Split(new string[] { " " }, StringSplitOptions.None);
                if (line.StartsWith("//"))
                {
                    yield return new WaitForSeconds(0);
                }
                if (line.StartsWith("wait"))
                {
                    yield return new WaitForSeconds(Convert.ToSingle(options[1]));
                }
                if (line.StartsWith("repeat"))
                {
                    FengGameManagerMKII.FGM.StartCoroutine(RunCmdtest(FilePath));
                    yield break;
                }
                if (line.StartsWith("stop"))
                {
                    FengGameManagerMKII.FGM.StopAllCoroutines();
                    yield break;
                }
                CMDHandler.TryHandle("/" + AnarchyExtensions.CommandFormat(line));
            }
            yield break;
        }
    }
}