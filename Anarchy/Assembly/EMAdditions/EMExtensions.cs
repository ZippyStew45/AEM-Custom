using System;
using System.Collections;
using System.Collections.Generic;
using Anarchy.Configuration;
using Optimization.Caching;
using RC;
using System.IO;
using System.Linq;
using Anarchy;
using Antis;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Anarchy.UI;
using System.Runtime.InteropServices;
using AoTTG.EMAdditions;
using System.Threading;
using AoTTG.EMAdditions.Scripts;

internal partial class FengGameManagerMKII
{
    private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
    private Dictionary<string, Dictionary<string, GameObject>> objects = new Dictionary<string, Dictionary<string, GameObject>>();
    private AssetBundle bundleCustomMap;
    private GameObject customMap;
    private string bundlePath = Path.Combine(Application.dataPath, @"CustomMap/");
    public float HeroGrav = 20f;
    public bool MCForceStats;
    public int MCGAS;
    public int MCBLA;
    public int MCSPD;
    public int MCACL;
    public float ImpactDeathSpeed;
    public bool ImpactDeathEnabled;

    [RPC]
    private void MedicRPC(bool medic, PhotonMessageInfo info)
    {
        Anarchy.UI.Chat.Add("Called");
        PhotonNetwork.player.SetCustomProperties(new Hashtable(){{PhotonPlayerProperty.medic, medic}});
    }

    [RPC]
    private void AemGunner(bool gunner, PhotonMessageInfo info)
    {
        Anarchy.UI.Chat.Add("Gunner Role Set to: " + gunner);
        PhotonNetwork.player.SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.Gunner, gunner } });
    }
    [RPC]
    private void AemSupply(bool Supply, PhotonMessageInfo info)
    {
        Anarchy.UI.Chat.Add("Gunner Role Set to: " + Supply);
        PhotonNetwork.player.SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.Supply, Supply } });
    }
    [RPC]
    private void AemBulder(bool Builder, PhotonMessageInfo info)
    {
        Anarchy.UI.Chat.Add("Gunner Role Set to: " + Builder);
        PhotonNetwork.player.SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.Builder, Builder } });
    }

    [RPC]
    private void ForceStatsRPC(bool force, int gas, int bla, int spd, int acl, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        MCForceStats = force;
        MCGAS = gas;
        MCBLA = bla;
        MCSPD = spd;
        MCACL = acl;

        var stats = IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat;
        stats.Acl = MCACL;
        stats.Bla = MCBLA;
        stats.Spd = MCSPD;
        stats.Gas = MCGAS;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable()
        {
            {PhotonPlayerProperty.statACL, acl},
            {PhotonPlayerProperty.statBLA, bla},
            {PhotonPlayerProperty.statGAS, gas},
            {PhotonPlayerProperty.statSPD, spd}
        };
        PhotonNetwork.player.SetCustomProperties(hash);
    }

    [RPC]
    private void SetImpactDeathRPC(bool enabled, float speed, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        ImpactDeathEnabled = enabled;
        ImpactDeathSpeed = speed;
    }

    [RPC]
    private void SetGravityRPC(float g, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        HeroGrav = g;
    }

    [RPC]
    private void SetDifficultyRPC(int Dif, PhotonMessageInfo info)
    {
        TITAN.instance.myDifficulty = Dif;
        IN_GAME_MAIN_CAMERA.Difficulty = Dif;
    }

    [RPC]
    private void SpawnPrimitiveRPC(string Object, Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        if (!info.Sender.Builder)
            return;
        string[] ItemSplit = Object.Split(' ');
        GameObject SpawnObj = new GameObject();
        switch (Object)
        {
            case string s when s.ToLower().StartsWith("cube"):
                SpawnObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case string s when s.ToLower().StartsWith("sphere"):
                SpawnObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case string s when s.ToLower().StartsWith("capsule"):
                SpawnObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
            case string s when s.ToLower().StartsWith("cylinder"):
                SpawnObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case string s when s.ToLower().StartsWith("quad"):
                SpawnObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                break;
            case string s when s.ToLower().StartsWith("plane"):
                SpawnObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                break;
            default:
                SpawnObj = Instantiate(RCManager.ZippyAssets.Load(ItemSplit[0]), position, rotation) as GameObject;
                goto skip;
                break;
        }
        SpawnObj.transform.localScale = new Vector3(10, 10, 10);
        SpawnObj.renderer.material.color = Color.gray;

        skip:

        SpawnObj.name = Object;
        SpawnObj.transform.position = position;
        SpawnObj.transform.rotation = rotation;
        SpawnObj.AddComponent<BuilderTag>();
        SpawnObj.layer = LayerMask.NameToLayer("Ground");
    }

    [RPC]
    private void DeletePrimitiveRPC(string obj, PhotonMessageInfo info)
    {
        if (!info.Sender.Builder)
            return;
        if (GameObject.Find(obj) == null)
            return;
        Destroy(GameObject.Find(obj));
    }


    [RPC]
    private void ShakeScreenRPC(float amount, float duration, float decay, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient) return;
        if (decay == 0f) decay = 0.95f;
        IN_GAME_MAIN_CAMERA.MainCamera.startShake( amount, duration, decay);
    }

    [RPC]
    private void NotifRPC(string message, float duration, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient) return;
        Anarchy.Notifications.NotifMessage.message.New(message, duration);
    }


    [RPC]
    private void DropGasRPC(string objname, Vector3 vec3, Quaternion quaternion, PhotonMessageInfo info)
    {
        var obj = UnityEngine.Object.Instantiate(RCManager.ZippyAssets.Load("GasCanister"), vec3, quaternion) as GameObject;
        obj.AddComponent<Rigidbody>();
        obj.name = objname;
        obj.layer = LayerMask.NameToLayer("Ground");
        obj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        obj.AddComponent<GasCollider>();
    }

    [RPC]
    private void DropbladeRPC(string objname, Vector3 vec3, Quaternion quaternion, PhotonMessageInfo info)
    {
        var obj = UnityEngine.Object.Instantiate(RCManager.ZippyAssets.Load("Blade"), vec3, quaternion) as GameObject;
        obj.AddComponent<Rigidbody>();
        obj.name = objname;
        obj.AddComponent<BoxCollider>();
        obj.layer = LayerMask.NameToLayer("Ground");
        obj.AddComponent<BladeCollider>();
    }

    [RPC]
    private void LightRPC(string option, string value, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        switch (option)
        {
            case "intensity":
                CacheGameObject.Find("mainLight").GetComponent<Light>().intensity = float.Parse(value);
                break;
            case "color":
                CacheGameObject.Find("mainLight").GetComponent<Light>().color = value.HexToColor();
                break;
        }
    }
    [RPC]
    private void FogRPC(string option, string value, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        switch (option)
        {
            case "start":
                RenderSettings.fogStartDistance = float.Parse(value);
                break;
            case "end":
                RenderSettings.fogEndDistance = float.Parse(value);
                break;
            case "density":
                RenderSettings.fogDensity = float.Parse(value);
                break;
            case "color":
                RenderSettings.fogColor = value.HexToColor();
                break;
            case "enabled":
                var result = Convert.ToBoolean(int.Parse(value));
                VideoSettings.MCFogOverride = result;
                RenderSettings.fog = result;
                break;
            case "mode":
                switch (value)
                {
                    case "ExponentialSquared":
                        RenderSettings.fogMode = FogMode.ExponentialSquared;
                        break;
                    case "Exponential":
                        RenderSettings.fogMode = FogMode.Exponential;
                        break;
                    case "Linear":
                        RenderSettings.fogMode = FogMode.Linear;
                        break;
                }
                break;
        }
        #region Added by Sysyfus for WaterVolume
        WaterVolume[] waterVolumes = FindObjectsOfType(typeof(WaterVolume)) as WaterVolume[];
        foreach (WaterVolume thisWaterVolume in waterVolumes)
        {
            thisWaterVolume.UpdateNormalFog(option, value, info);
        }
        #endregion
    }

    [RPC]
    public void FlareColour(Vector3 pos, Quaternion rot, float r, float g, float b, bool flash, PhotonMessageInfo info)
    {
        Color col = new Color(r, g, b);
        var obj = (GameObject)Instantiate(CacheResources.Load("FX/flareBullet1"), pos, rot);
        if (flash == true)
        {
            Light bulbComp = obj.AddComponent<Light>();
            bulbComp.renderMode = LightRenderMode.ForcePixel;
            bulbComp.range = 500f;
            bulbComp.intensity = 1f;
            bulbComp.color = col;
        }
        obj.GetComponent<ParticleSystem>().startColor = col;
        obj.GetComponent<FlareMovement>().dontShowHint();
    }

    [RPC]
    public void SetFlashLight(int ID, int Toggle, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        GameObject Player1 = PhotonPlayer.Find(ID).GameObject;
        if (Toggle == 1)
        {
            if (Player1.GetComponent<Light>() != null)
            {
                Player1.GetComponent<Light>().enabled = true;
                return;
            }
            Light bulbComp = Player1.AddComponent<Light>();
            bulbComp.renderMode = LightRenderMode.ForcePixel;
            bulbComp.range = 100f;
            bulbComp.intensity = 1f;
        }
        else
        {
            Player1.GetComponent<Light>().enabled = false;
        }
    }

    [RPC]
    public void SetDayLevel(float r, float g, float b, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
            return;
        FengColor.Custom = new Color(r, g, b);
        IN_GAME_MAIN_CAMERA.MainCamera.setDayLight(DayLight.Custom);
    }

    [RPC]
    private void SpawnWagon(int horseId, bool refill, string fileName, int id, PhotonMessageInfo info)
    {
        if (!info.Sender.Wagoneer)
            return;
        var h = PhotonView.Find(horseId);
        var vector3 = (-h.transform.forward) * 14;
        var vector4 = h.transform.position;
        vector4 += h.transform.right * 6.5f;
        vector4.y -= 0.1f;
        var obj1 = RCManager.EMAssets.Load(fileName) as GameObject;
        obj1.transform.localScale = new Vector3(2, 2, 2);
        var obj2 = Instantiate(obj1, vector3 + vector4,
            Quaternion.Euler(-1f * h.transform.rotation.eulerAngles.x, h.transform.rotation.eulerAngles.y + 180, 0)) as GameObject; //Changed by Sysyfus to accomodate tilted horse at time of spawn
        obj2.transform.SetParent(h.transform, true);
        foreach (var comp in obj2.GetComponentsInChildren<Collider>())
        {
            comp.gameObject.layer = LayerMask.NameToLayer("Ground");
        }
        obj2.gameObject.layer = LayerMask.NameToLayer("Ground");
        GameObject go = obj2.transform.FindChild("SupplyWagon1_1").gameObject;
        WheelRotate wr = go.AddComponent<WheelRotate>();
        wr.FRWheelT = go.transform.FindChild("Wheel3/Mesh7");
        wr.FLWheelT = go.transform.FindChild("Wheel4/Mesh8");
        wr.BRWheelT = go.transform.FindChild("Wheel2/Mesh6");
        wr.BLWheelT = go.transform.FindChild("Wheel1/Mesh3");
        if (refill)
        {
            var v3 = obj2.transform.position;
            v3.y += 2;
            v3 += (obj2.transform.right * 6.5f);
            v3 -= (obj2.transform.forward * 5f);
            var obj3 = Resources.Load("aot_supply") as GameObject;
            var obj4 =
                Instantiate(obj3, v3, Quaternion.Euler(-1f * obj2.transform.eulerAngles.x, obj2.transform.eulerAngles.y + 180, 0)) as GameObject; //Changed by Sysyfus to accomodate tilted horse at time of spawn
            obj4.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            obj4.transform.parent = obj2.transform;
            obj4.AddComponent<WagonAutoFill>();
        }
        h.GetComponent<Horse>().wag = obj2;
        h.GetComponent<Horse>().Wagon = true;
        obj2.transform.FindChild("SupplyWagon1_1/Cart1").gameObject.AddComponent<Wagon>();
        obj2.AddComponent<PhotonView>().viewID = id;
    }

    [RPC]
    private void SpawnWagon2(int horseId, bool refill, string fileName, int id, PhotonMessageInfo info)
    {
        if (!info.Sender.Wagoneer)
            return;
        var h = PhotonView.Find(horseId);
        var vector3 = (-h.transform.forward) * 6;
        var vector4 = h.transform.position;
        vector4.y += 0.775f;
        var obj1 = RCManager.ZippyAssets.Load("AEMWagon") as GameObject;
        obj1.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        obj1.AddComponent<Rigidbody>().isKinematic = true;
        var obj2 = Instantiate(obj1, vector3 + vector4, Quaternion.Euler(-1f * h.transform.rotation.eulerAngles.x, h.transform.rotation.eulerAngles.y, 0)) as GameObject;
        obj2.transform.SetParent(h.transform, true);

        foreach (var comp in obj2.GetComponentsInChildren<Collider>())
        {
            comp.gameObject.layer = LayerMask.NameToLayer("Ground");
        }
        obj2.gameObject.layer = LayerMask.NameToLayer("Ground");
        GameObject go = obj2.transform.FindChild("Wheels").gameObject;
        WheelRotate2 wr = go.AddComponent<WheelRotate2>();
        wr.FRWheelT = go.transform.FindChild("WheelMeshFront");
        wr.FLWheelT = go.transform.FindChild("WheelMeshFront");
        wr.BRWheelT = go.transform.FindChild("WheelMeshBack");
        wr.BLWheelT = go.transform.FindChild("WheelMeshBack");
        if (refill)
        {
            var v3 = obj2.transform.position;
            v3.y += 0.2f;
            v3 += (obj2.transform.forward * 1.55f);
            var obj3 = Resources.Load("aot_supply") as GameObject;
            var obj4 = Instantiate(obj3, v3, Quaternion.Euler(-1f * obj2.transform.eulerAngles.x, obj2.transform.eulerAngles.y, 0)) as GameObject;
            obj4.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            obj4.transform.parent = obj2.transform;
            obj4.AddComponent<WagonAutoFill>();
        }
        h.GetComponent<Horse>().wag = obj2;
        h.GetComponent<Horse>().Wagon = true;
        obj2.transform.FindChild("Body/BodyMesh").gameObject.AddComponent<Wagon>();
        obj2.AddComponent<PhotonView>().viewID = id;
    }

    [RPC]
    private void DisconnectWagon(int horseId, Vector3 pos, Quaternion rot, PhotonMessageInfo info)
    {
        if (!info.Sender.Wagoneer)
            return;
        var h = PhotonView.Find(horseId).GetComponent<Horse>();
        h.wag.transform.position = pos;
        h.wag.transform.rotation = rot;
        h.wag.transform.parent = null;
        h.Wagon = false;
        if (BasePV.IsMine)
        {
            var vector4 = h.transform.position;
            vector4.y -= 0.775f;
        }
    }

    [RPC]
    private void ReconnectWagon(int horseID, PhotonMessageInfo info)
    {
        if (!info.Sender.Wagoneer)
            return;
        var h = PhotonView.Find(horseID);
        var wag = h.GetComponent<Horse>().wag;
        var vector3 = (-h.transform.forward) * 14;
        var vector4 = h.transform.position;
        vector4 += h.transform.right * 6.5f;
        vector4.y -= 0.1f;
        wag.transform.position = vector3 + vector4;
        wag.transform.rotation = Quaternion.Euler(0, h.transform.rotation.eulerAngles.y + 180, 0);
        wag.transform.parent = h.transform;
        h.GetComponent<Horse>().Wagon = true;
    }

    [RPC]
    private void ReconnectWagon2(int horseID, PhotonMessageInfo info)
    {
        if (!info.Sender.Wagoneer)
            return;
        var h = PhotonView.Find(horseID);
        var vector3 = (-h.transform.forward) * 6;
        var vector4 = h.transform.position;
        vector4.y += 0.775f;
        var wag = h.GetComponent<Horse>().wag;
        wag.transform.position = vector3 + vector4;
        wag.transform.rotation = Quaternion.Euler(0, h.transform.rotation.eulerAngles.y, 0);
        wag.transform.parent = h.transform;
        h.GetComponent<Horse>().Wagon = true;
    }

    public void SPTitan(int type, float size, int health, float speed, int count, int chaseDistance, int attackWait, float posX, float posY, float posZ, bool lockAxis, bool faker = false, bool RockThrow = false, bool speedtitan = false, bool stalker = false, string bodySkinLink = "", string eyeSkinLink = "", float animationSpeed = 1f)
    {
        var position = new Vector3(posX, posY, posZ);
        var rotation = new Quaternion(0f, 0f, 0f, 0f);
        for (var i = 0; i < count; i++)
        {
            TITAN t = SpawnTitanCommand(type, position, rotation);
            t.ResetLevel(size);
            t.hasSetLevel = true;
            t.maxHealth = health;
            t.currentHealth = health;
            t.CustomAttackWait = attackWait;
            t.CustomSpeed = speed;
            t.rigidbody.freezeRotation = lockAxis;
            t.chaseDistance = chaseDistance;
            if (RockThrow) t.gameObject.AddComponent<PunkRockTag>();
            if (speedtitan) t.gameObject.AddComponent<SpeedTitan>();
            if (stalker) t.gameObject.AddComponent<StalkerTitan>();
            if (faker) t.gameObject.AddComponent<FakerTitan>();
            t.BasePV.RPC("AniSpeed", PhotonTargets.AllBuffered, animationSpeed);
            t.BasePV.RPC("loadskinRPC", PhotonTargets.AllBuffered, bodySkinLink, eyeSkinLink);
            //t.SetAbnormalTypeCommand((AbnormalType)type);
        }
    }
    [RPC]
    private void EMCustomMapRPC(string localPath, string prefabName, Vector3 position, Quaternion rotation, PhotonMessageInfo info = null)
    {
        if (info.Sender.IsMasterClient)
        {
            //threaded for optimzation
            Thread simulationThreadenter = new Thread(() => Loadmapthread(localPath, prefabName, position, rotation));
            simulationThreadenter.Start();
        }
    }
    //threaded for optimzation
    private void Loadmapthread(string localPath, string prefabName, Vector3 position, Quaternion rotation)
    {
        if (bundleCustomMap != null) bundleCustomMap.Unload(true);
        IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<TiltShift>().enabled = true;
        base.StartCoroutine(LoadCustomMap(localPath, prefabName, position, rotation));
        Thread.CurrentThread.Abort();
    }

    [RPC]
    private void LoadBundleRPC(string bundleName, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            StartCoroutine(LoadBundle(bundleName));
        }
    }

    [RPC]
    private void LoadObjectRPC(string bundleName, string objectName, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            StartCoroutine(LoadObject(bundleName, objectName));
        }
    }

    [RPC]
    private void SpawnObjectRPC(string bundleName, string objectName, Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            StartCoroutine(SpawnObject(bundleName, objectName, position, rotation));
        }
    }

    [RPC]
    private void MoveObjectRPC(string name, Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            StartCoroutine(MoveObject(name, position, rotation));
        }
    }

    [RPC]
    private void DestroyObjectsRPC(string[] names, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            StartCoroutine(DestroyObjects(names));
        }
    }
    [RPC]
    private IEnumerator LoadCustomMap(string localPath, string prefabName, Vector3 position, Quaternion rotation)
    {
        string path = String.Concat(bundlePath, $"{localPath}.unity3d");
        byte[] data = System.IO.File.ReadAllBytes(path);
        AssetBundleCreateRequest assets = AssetBundle.CreateFromMemory(data);
        yield return assets;
        bundleCustomMap = assets.assetBundle;
        customMap = ((GameObject)assets.assetBundle.Load(prefabName));
        base.StartCoroutine(SpawnCustomMap(position, rotation));
    }

    private IEnumerator LoadObject(string bundleName, string objectName)
    {
        while (!bundles.ContainsKey(bundleName))
        {
            yield return null;
        }
        if (bundles.ContainsKey(bundleName))
        {
            objects[bundleName].Add(objectName, bundles[bundleName].Load(objectName) as GameObject);
        }
    }
    private IEnumerator SpawnCustomMap(Vector3 position, Quaternion rotation)
    {
        while (customMap == null)
        {
            yield return null;
        }
        Anarchy.UI.Chat.Add(string.Concat("Instantiating a Unity map: Position - ", position, "; Rotation - ", rotation));
        customMap = (GameObject)UnityEngine.Object.Instantiate(customMap, position, rotation);
        foreach (Terrain t in Terrain.activeTerrains)
        {
            if (Settings.TreeBillboardDistanceOverride) t.treeBillboardDistance = Settings.TreeBillboardDistance;
            if (Settings.BaseMapDistanceOverride) t.basemapDistance = Settings.BaseMapDistance;
            if (Settings.TreeDistanceOverride) t.treeDistance = Settings.TreeDistance;
            if (Settings.DetailObjectDistanceOverride) t.detailObjectDistance = Settings.DetailObjectDistance;
        }
        IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<TiltShift>().enabled = VideoSettings.Blur.Value;
    }

    private IEnumerator SpawnObject(string bundleName, string objectName, Vector3 pos, Quaternion rot)
    {
        while (!objects[bundleName].ContainsKey(objectName))
        {
            yield return null;
        }
        Instantiate(objects[bundleName][objectName], pos, rot);
    }

    private IEnumerator LoadBundle(string bundle)
    {
        byte[] data = File.ReadAllBytes(bundlePath + bundle + ".unity3d");
        AssetBundleCreateRequest assets = AssetBundle.CreateFromMemory(data);
        yield return assets;
        if (assets != null)
        {
            bundles.Add(bundle, assets.assetBundle);
            objects.Add(bundle, new Dictionary<string, GameObject>());
        }
    }

    private IEnumerator MoveObject(string name, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GameObject.Find(name);
        while (obj == null)
        {
            obj = GameObject.Find(name);
            yield return null;
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
    }

    private IEnumerator DestroyObjects(string[] names)
    {
        List<GameObject> objects = new List<GameObject>();
        while (objects.Count == 0)
        {
            foreach(var obj in FindObjectsOfType<GameObject>())
            {
                if (names.Contains(obj.name))
                {
                    objects.Add(obj);
                }
            }

            yield return null;
        }

        foreach (var obj in objects)
        {
            Destroy(obj);
        }
    }

    private static System.Random randomm = new System.Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[randomm.Next(s.Length)]).ToArray());
    }

    /*
    public IEnumerator ExecuteCommandList(string path)
    {
        var commandList = File.ReadAllLines(path);
        foreach (string line in commandList)
        {
            if (line.StartsWith("WAIT"))
            {
                yield return new WaitForSeconds(float.Parse(line.Substring(5)));
            }
            else
            {
                if (RC.RCManager.RCEvents.ContainsKey("OnChatInput"))
                {
                    string key = (string) RC.RCManager.RCVariableNames["OnChatInput"];
                    var collection = RC.RCManager.stringVariables;
                    if (collection.ContainsKey(key))
                    {
                        collection[key] = line;
                    }
                    else
                    {
                        collection.Add(key, line);
                    }

                    ((RCEvent) RC.RCManager.RCEvents["OnChatInput"]).checkEvent();
                }

                Anarchy.UI.Chat.CMDHandler.TryHandle(line);
            }
        }
    }
    */
}