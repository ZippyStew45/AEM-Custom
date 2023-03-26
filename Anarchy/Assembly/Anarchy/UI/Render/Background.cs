using System;
using Mono.Security;
using UnityEngine;

namespace Anarchy.UI
{
    public class Background : GUIBase
    {
        private Rect screenRect;
        private GUIStyle style;
        private Texture2D texture;
        private MovieTexture movie;

        public Background() : base("Wallpapers", GUILayers.Background)
        {
        }

        protected override void OnDisable()
        {
            texture = null;
            movie = null;
            style = null;
            AnarchyManager.MainMenu.Disable();
        }
        private string GetAspectRatio()
        {
            float val = (float) Screen.width / Screen.height;
            if (val > 1.3 && val < 1.4) return "Box";
            if (val > 1.7 && val < 1.8) return "Wide";
            if (val > 2.3) return "Ultrawide";
            return "Wide";
        }
        protected override void OnEnable()
        {
            string[] allImages = System.IO.Directory.GetFiles(Directory + GetAspectRatio());
            if (allImages.Length == 0)
            {
                texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.SetPixel(0, 0, new Color(0f, 36f / 255f, 36f / 255f, 1f));
                texture.Apply();
                return;
            }
            var random = allImages[UnityEngine.Random.Range(0, allImages.Length)].Substring(Directory.Length);
            if (random.EndsWith(".ogv"))
            {
                movie = LoadVideo(random);
                if (movie != null)
                {
                    movie.loop = true;
                    movie.Play();
                }
            }
            else
            {
                texture = LoadTexture(random, "");
            }
            screenRect = new Rect(0f, 0f, Style.ScreenWidth, Style.ScreenHeight);
            style = Helper.CreateStyle(TextAnchor.LowerCenter, FontStyle.Bold, 25, false, new Color(0.95f, 0.95f, 0.95f));
            style.richText = true;
            style.font = AnarchyAssets.Load<Font>(Style.FontName);
            AnarchyManager.MainMenu.Enable();
        }

        protected internal override void Draw()
        {
            if (movie != null)
            {
                UnityEngine.GUI.DrawTexture(screenRect, movie);
            }
            else
            {
                UnityEngine.GUI.DrawTexture(screenRect, texture);
            }
            UnityEngine.GUI.Label(screenRect, string.Format(UIMainReferences.VersionShow, AnarchyManager.AnarchyVersion.ToString()), style);
        }
    }
}