using System;
using System.Diagnostics;
using UnityEngine;
using Anarchy;
using System.Collections;
using System.Collections.Generic;
using AoTTG.EMAdditions.Sounds;

public sealed class UIMainReferences : MonoBehaviour
{
    private static bool isGAMEFirstLaunch = true;
    public const string VersionShow = "AEM Version {1} \nAnarchy({0})";
    public static string ConnectField = "01042015"; 
    public static string UpdaterPath = Environment.CurrentDirectory + "\\AEMUpdater.exe";
    public static UIMainReferences Main;
    public GameObject panelCredits;
    public GameObject PanelDisconnect;
    public GameObject panelMain;
    public GameObject PanelMultiJoinPrivate;
    public GameObject PanelMultiPWD;
    public GameObject panelMultiROOM;
    public GameObject panelMultiSet;
    public GameObject panelMultiStart;
    public GameObject PanelMultiWait;
    public GameObject panelOption;
    public GameObject panelSingleSet;
    public GameObject PanelSnapShot;

    private void Awake()
    {
        Main = this;
    }

    private void Start()
    {
        GameObject.Find("VERSION").GetComponent<UILabel>().text = "Loading...";
        NGUITools.SetActive(this.panelMain, false);
        if (isGAMEFirstLaunch)
        {
            isGAMEFirstLaunch = false;
            GameObject input = (GameObject)Resources.Load("InputManagerController");
            input.GetComponent<FengCustomInputs>().enabled = false;
            input.AddComponent<Anarchy.InputManager>();
            var inputs = (GameObject)Instantiate(input);
            inputs.name = "InputManagerController";
            DontDestroyOnLoad(inputs);
            new GameObject("AnarchyManager").AddComponent<Anarchy.AnarchyManager>();

            ProcessStartInfo startInfo = new ProcessStartInfo(UpdaterPath);
            Process.Start(startInfo);
        }
        Anarchy.Network.NetworkManager.TryRejoin();
        UIMainReferences.LoadDictionaryOfSounds();
        AudioManagerInit();
    }


    public void AudioManagerInit()
    {
        //foreach (KeyValuePair<string, List<GameObject>> pair in clothCache)
        foreach (KeyValuePair<string, AudioClip> dictionary_of_sound in AudioManager.dictionary_of_sounds)
            StartCoroutine(LoadSoundFromAudioManager(dictionary_of_sound.Key));
    }

    public IEnumerator LoadSoundFromAudioManager(string name)
    {
        string url = "File://" + Application.dataPath + "/Resources/Sounds/audio_" + name + ".wav";
        using (WWW iteratorVariable2 = new WWW(url))
        {
            AudioClip audioClip = iteratorVariable2.GetAudioClip(false, true);
            while (!audioClip.isReadyToPlay)
            {
                yield return (object)new WaitForEndOfFrame();
            }
            AudioManager.dictionary_of_sounds[name] = iteratorVariable2.audioClip;
            AudioManager.List_string_of_loaded_sounds.Add(name);
        }
    }

    public static void LoadDictionaryOfSounds() //adds clips that get loaded in fgm
    {
        if (!AudioManager.dictionary_of_sounds.ContainsKey("AIQuoted")) AudioManager.dictionary_of_sounds.Add("AIQuoted", AudioManager.audio_Quoted);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("horse_gallop")) AudioManager.dictionary_of_sounds.Add("horse_gallop", AudioManager.audio_horse_gallop);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("player_footsteps")) AudioManager.dictionary_of_sounds.Add("player_footsteps", AudioManager.audio_player_footsteps);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("rope")) AudioManager.dictionary_of_sounds.Add("rope", AudioManager.audio_hook_launch);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("slash_titan")) AudioManager.dictionary_of_sounds.Add("slash_titan", AudioManager.audio_slash_titan);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("rope_hit_enemy")) AudioManager.dictionary_of_sounds.Add("rope_hit_enemy", AudioManager.audio_hook_hit_enemy);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("rope_hit_static_objects")) AudioManager.dictionary_of_sounds.Add("rope_hit_static_objects", AudioManager.audio_rope_hit_staticObjects);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("gas_burst")) AudioManager.dictionary_of_sounds.Add("gas_burst", AudioManager.audio_gas_burst);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("gas")) AudioManager.dictionary_of_sounds.Add("gas", AudioManager.audio_gas);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("shootflare")) AudioManager.dictionary_of_sounds.Add("shootflare", AudioManager.audio_shoot_flare);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("refill")) AudioManager.dictionary_of_sounds.Add("refill", AudioManager.audio_refill);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("reel_in")) AudioManager.dictionary_of_sounds.Add("reel_in", AudioManager.audio_reel_in);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("reel_out")) AudioManager.dictionary_of_sounds.Add("reel_out", AudioManager.audio_reel_out);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("slide")) AudioManager.dictionary_of_sounds.Add("slide", AudioManager.audio_slide);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("blade_reload")) AudioManager.dictionary_of_sounds.Add("blade_reload", AudioManager.audio_blade_reload);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("guns_reload")) AudioManager.dictionary_of_sounds.Add("guns_reload", AudioManager.audio_guns_reload);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("guns_shoot")) AudioManager.dictionary_of_sounds.Add("guns_shoot", AudioManager.audio_guns_shoot);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("blade_broken")) AudioManager.dictionary_of_sounds.Add("blade_broken", AudioManager.audio_blade_broken);
        if (!AudioManager.dictionary_of_sounds.ContainsKey("player_titan_die")) AudioManager.dictionary_of_sounds.Add("player_titan_die", AudioManager.audio_player_titan_die);
    }
}
