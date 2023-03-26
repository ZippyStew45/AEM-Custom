using Anarchy.Configuration.Storage;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.Configuration
{
    /// <summary>
    /// Main settings storage
    /// </summary>
    public class Settings
    {
        private static List<ISetting> allSettings;
        private static object locker = new object();
        public static IDataStorage Storage;

        public static BoolSetting DisableHookArrrows = new BoolSetting("DisableHookArrows", false);
        public static BoolSetting InfiniteGasPvp = new BoolSetting("InfiniteGasPvp", false);

        public static BoolSetting HideName = new BoolSetting("HideName", true);
        public static BoolSetting RemoveColors = new BoolSetting("RemoveColors", false);

        public static BoolSetting RacingTimerOnCrosshair = new BoolSetting("CrosshairRacingTimer", false);
        public static BoolSetting BombTimerOnCrosshair = new BoolSetting("CrosshairBombTimer", false);

        public static BoolSetting BodyLeanEnabled = new BoolSetting("BodyLeanEnabled", true);


        public static BoolSetting WagonRefill = new BoolSetting("WagonRefill", true);
        public static BoolSetting BaseMapDistanceOverride = new BoolSetting("BaseMapOverride", false);
        public static FloatSetting BaseMapDistance = new FloatSetting("BaseMapDistance", 0f);
        public static BoolSetting DetailObjectDistanceOverride = new BoolSetting("DetailObjectOverride", false);
        public static FloatSetting DetailObjectDistance = new FloatSetting("DetailObjectDistance", 0f);
        public static BoolSetting TreeDistanceOverride = new BoolSetting("TreeDistanceOverride", false);
        public static FloatSetting TreeDistance = new FloatSetting("TreeDistance", 0f);
        public static BoolSetting TreeBillboardDistanceOverride = new BoolSetting("TreeBillboardOverride", false);
        public static FloatSetting TreeBillboardDistance = new FloatSetting("TreeBillboardDistance", 0f);

        public static BoolSetting InvertY = new BoolSetting("InvertY", false);
        public static BoolSetting Snapshots = new BoolSetting("Snapshots", false);
        public static BoolSetting Interpolation = new BoolSetting("InterpolateEnabled", true);
        public static BoolSetting SnapshotsInGame = new BoolSetting("SnapshotsInGame", false);
        public static BoolSetting Speedometer = new BoolSetting("Speedometer", false);
        public static BoolSetting Minimap = new BoolSetting("Minimap", false);

        public static IntSetting CameraMode = new IntSetting("CameraMode", 0);
        public static IntSetting SnapshotsDamage = new IntSetting("SnapshotsDamage", 0);
        public static IntSetting SpeedometerType = new IntSetting("SpeedometerType", 0);

        public static FloatSetting CameraDistance = new FloatSetting("CameraDistance", 1f);
        public static FloatSetting MouseSensivity = new FloatSetting("MouseSensivity", 0.5f);
        public static FloatSetting SoundLevel = new FloatSetting("SoundLevel", 1f);
        public static void AddSetting(ISetting set)
        {
            lock (locker)
            {
                if (allSettings == null)
                {
                    allSettings = new List<ISetting>();
                }

                allSettings.Add(set);
            }
        }

        public static void Apply()
        {
            AudioListener.volume = SoundLevel.Value;
            IN_GAME_MAIN_CAMERA.sensitivityMulti = MouseSensivity.Value;
            IN_GAME_MAIN_CAMERA.cameraDistance = 0.3f + CameraDistance.Value; ;
            IN_GAME_MAIN_CAMERA.invertY = InvertY.Value ? -1 : 1;
        }

        public static void Clear()
        {
            if (Storage == null)
            {
                CreateStorage();
            }

            Storage.Clear();
        }

        public static void CreateStorage()
        {
            switch (PlayerPrefs.GetInt("AnarchyDataStorage", 1))
            {
                case 0:
                    Storage = new PrefStorage();
                    break;

                case 1:
                    Storage = new AnarchyStorage();
                    break;

                default:
                    Storage = new PrefStorage();
                    break;
            }
        }

        public static void Load()
        {
            if (allSettings == null)
            {
                allSettings = new List<ISetting>();
            }

            if (Storage == null)
            {
                CreateStorage();
            }

            lock (locker)
            {
                for (int i = 0; i < allSettings.Count; i++)
                {
                    allSettings[i].Load();
                }
            }
        }

        public static void RemoveSetting(ISetting set)
        {
            lock (locker)
            {
                if (allSettings.Contains(set))
                {
                    allSettings.Remove(set);
                }
            }
        }

        public static void Save()
        {
            if (allSettings == null)
            {
                allSettings = new List<ISetting>();
            }

            lock (locker)
            {
                for (int i = 0; i < allSettings.Count; i++)
                {
                    allSettings[i].Save();
                }
            }
            Storage.Save();
        }
    }
}
