using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using System.Net.NetworkInformation;
using System.Linq;

namespace Updater
{
    internal class MyUpdater : MonoBehaviour
    {
        public static GUIStyle FontStyle;
        public static Texture2D ColorTexture;
        public static Texture2D ColorTexture2;
        public static Texture2D HookTexture3;
        public static float ColorFloats0 = 50.0f;
        public static float ColorFloats1 = 60.0f;
        public static float ColorFloats2 = 10.0f;
        public static float AlternativeColorFloats0 = 50.0f;
        public static float AlternativeColorFloats1 = 60.0f;
        public static float AlternativeColorFloats2 = 10.0f;
        public static float HookColorFloats0 = 0f;
        public static float HookColorFloats1 = 0f;
        public static float HookColorFloats2 = 0f;
        private WWW assembly;
        private static bool assemblyloaded;
        private WWW Antis;
        private static bool Antisloaded;
        private static bool Relaunching;
        private static bool Relaunched;
        public static bool VersionChecked;


        private static string DllDirectory = Environment.CurrentDirectory + "\\AoTTG_Data\\Managed\\";


        public Texture2D progressColor;
        public Texture2D textureBackgroundBlack;


        public static int UpdateType = 0;
        public static string UpdateStatus = "";


        public static bool DownloadDLL = false;
        public static string ModVersion = Anarchy.AnarchyManager.CustomVersion;


        private WWW bat1;
        private static bool bat1loaded;
        private WWW bat2;
        private static bool bat2loaded;
        private WWW delExe;
        private static bool delExeloaded;

        public static void GroupBox(string name, int xMax, int yMax)
        {
            Vector2 strBounds = PanelMain.TextBounds(name);
        }

        public void OnGUI()
        {
            if (DownloadDLL == true)
            {
                if (!this.assembly.isDone)
                {
                    GUI.backgroundColor = Color.black;
                    GUI.DrawTexture(new Rect((float)Screen.width / 2f - 155f, (float)Screen.height / 2f - 10f, 310f, 60f), this.textureBackgroundBlack);
                    GUI.contentColor = Color.green;
                    GUI.Box(new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f - 5f, 300f, 25f), "Assembly is downloading " + Mathf.Round(this.assembly.progress * 100f).ToString() + "%");
                    GUI.DrawTexture(new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f + 15f, 300f * Mathf.Clamp(this.assembly.progress, System.Math.Min(0f, this.assembly.progress), System.Math.Max(0f, this.assembly.progress)), 25f), this.progressColor);
                }
                if (this.assembly.isDone)
                {
                    MyUpdater.assemblyloaded = true;
                }

                if (!this.Antis.isDone)
                {
                    GUI.backgroundColor = Color.black;
                    GUI.DrawTexture(new Rect((float)Screen.width / 2f - 155f, (float)Screen.height / 2f - 10f, 310f, 60f), this.textureBackgroundBlack);
                    GUI.contentColor = Color.green;
                    GUI.Box(new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f - 5f, 300f, 25f), "Photon is downloading " + Mathf.Round(this.Antis.progress * 100f).ToString() + "%");
                    GUI.DrawTexture(new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f + 15f, 300f * Mathf.Clamp(this.Antis.progress, System.Math.Min(0f, this.Antis.progress), System.Math.Max(0f, this.Antis.progress)), 25f), this.progressColor);
                }
                if (this.Antis.isDone)
                {
                    MyUpdater.Antisloaded = true;
                }

                if (MyUpdater.Antisloaded == true && MyUpdater.assemblyloaded == true && MyUpdater.Relaunched == false)
                {
                    {
                        MyUpdater.Relaunching = true;
                        base.StartCoroutine(Relaunch());
                        MyUpdater.Relaunched = true;
                    }
                }
            }
        }

        string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        
        IEnumerator DownloadModule()
        {
            WWW wwwAssembly = new WWW("https://www.dropbox.com/s/93hqhh6wymm14ry/Assembly-CSharp.dll?dl=1"); //change to that assembly.txt
            this.assembly = wwwAssembly;
            yield return this.assembly;
            System.IO.File.WriteAllBytes(DllDirectory + "Assembly-CSharp.dll", wwwAssembly.bytes);
            MyUpdater.assemblyloaded = true;

            WWW wwwAntis = new WWW("https://www.dropbox.com/s/q3c1bmb00hs38ru/Photon3Unity3D.dll?dl=1");
            this.Antis = wwwAntis;
            yield return this.Antis;
            System.IO.File.WriteAllBytes(DllDirectory + "Antis.dll", wwwAntis.bytes);
            MyUpdater.Antisloaded = true;
        }

        IEnumerator Relaunch()
        {
            if (Relaunching == true && Relaunched == false)
            {
                UpdateType = 3;
                UpdateStatus = "Version is current...Relaunching..";
                yield return new WaitForSeconds(3f);
                Process.Start(Environment.CurrentDirectory + "\\AoTTG.exe");
                Application.Quit();
            }
        }

        IEnumerator CheckVersion()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (VersionChecked == false)
                {
                    WWW WWWUpdate = new WWW("https://www.dropbox.com/s/w5zv01q37s05th5/my%20mod%20version.txt?dl=1");
                    yield return WWWUpdate;

                    if (!string.IsNullOrEmpty(WWWUpdate.error))
                    {
                        UnityEngine.Debug.Log(WWWUpdate.error);
                        UpdateType = 2;
                        UpdateStatus = "Can't download, no internet connection";
                        yield break;
                        //MyUpdater.AddDebugLine(1, WWWUpdate.error);
                    }

                    string[] UpdateString = WWWUpdate.text.Split(new char[] { ' ' });
                    if (ModVersion != UpdateString[0])
                    {
                        DownloadDLL = true;
                        UpdateType = 2;
                        UpdateStatus = "Downloading new version... please wait";
                        UnityEngine.Debug.Log(WWWUpdate.text);
                    }
                    else
                    {
                        UpdateType = 3;
                        UpdateStatus = "Version is current...";
                        VersionChecked = true;
                        yield return new WaitForSeconds(1f);
                        UpdateStatus = "<color=#32CD32>Welcome to AEM!</color>";
                    }
                    if (DownloadDLL == true) base.StartCoroutine(DownloadModule());
                }
            }
            else
            {
                UpdateType = 2;
                UpdateStatus = "Connection is not available";
            }
        }
        
        IEnumerator WaitLoad()
        {
            yield return new WaitForSeconds(2.0f);
            UpdateType = 1;
            UpdateStatus = "Checking mod version...";
            base.StartCoroutine(CheckVersion());
        }

        void Start()
        {
            if (this.textureBackgroundBlack == null)
            {
                this.textureBackgroundBlack = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                this.textureBackgroundBlack.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
                this.textureBackgroundBlack.Apply();
            }
            if (this.progressColor == null)
            {
                this.progressColor = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                this.progressColor.SetPixel(0, 0, new Color(0.08f, 0.3f, 0.4f, 1f));
                this.progressColor.Apply();
            }

            // this.BytesChecks();
            base.StartCoroutine(WaitLoad());
            ColorTexture = new Texture2D(1, 1);
            ColorTexture.SetPixel(1, 1, new Color(0.0f, 0.0f, 1.0f, 1.0f));
            ColorTexture.Apply();

            ColorTexture2 = new Texture2D(1, 1);
            ColorTexture2.SetPixel(1, 1, new Color(0.0f, 0.0f, 1.0f, 1.0f));
            ColorTexture2.Apply();

            HookTexture3 = new Texture2D(1, 1);
            HookTexture3.SetPixel(1, 1, new Color(0.0f, 0.0f, 1.0f, 1.0f));
            HookTexture3.Apply();
        }
    }
}
