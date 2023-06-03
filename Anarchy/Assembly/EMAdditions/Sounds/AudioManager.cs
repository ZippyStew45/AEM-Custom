using System.Collections.Generic;
using UnityEngine;

namespace AoTTG.EMAdditions.Sounds
{
    internal class AudioManager : MonoBehaviour
    {
        public static List<string> List_string_of_loaded_sounds = new List<string>();

        //the clips, used in uimainref, also only rope and blade slash neck are used from here
        public static Dictionary<string, AudioClip> dictionary_of_sounds = new Dictionary<string, AudioClip>();

        //hooks
        public static AudioClip audio_hook_launch; //this gets loaded only in the dictionary and hero awake //done

        public static AudioClip audio_hook_hit_enemy; //done
        public static AudioSource AudioSource_hook_hit_enemy;

        public static AudioClip audio_rope_hit_staticObjects; //done
        public static AudioSource AudioSource_rope_hit_staticObjects;


        public static AudioClip audio_player_titan_die; //this gets loaded only in the dictionary and hero awake //done

        //gas
        public static AudioClip audio_gas; //done
        public static AudioSource AudioSource_gas; 

        public static AudioClip audio_gas_burst; //done
        public static AudioSource AudioSource_gas_burst; 

        //blades and refill
        public static AudioClip audio_blade_reload; //done
        public static AudioSource AudioSource_blade_reload; 

        public static AudioClip audio_blade_broken; //done
        public static AudioSource AudioSource_blade_broken; 

        public static AudioClip audio_refill; //done
        public static AudioSource AudioSource_refill;

        public static AudioClip audio_slash_titan; //this gets loaded only in the dictionary and hero awake //done

        //guns shoot and reload
        public static AudioClip audio_guns_reload; //done
        public static AudioSource AudioSource_guns_reload; 

        public static AudioClip audio_guns_shoot; //done
        public static AudioSource AudioSource_guns_shoot; 

        //reeling and slide
        public static AudioClip audio_reel_in; //done
        public static AudioSource AudioSource_reel_in;

        public static AudioClip audio_reel_out;//done
        public static AudioSource AudioSource_reel_out;

        public static AudioClip audio_slide; //done
        public static AudioSource AudioSource_slide;

        //walking sounds
        public static AudioClip audio_horse_gallop; //done
        public static AudioSource AudioSource_horse_gallop; 

        public static AudioClip audio_player_footsteps; //done
        public static AudioSource AudioSource_player_footsteps; 

        public static AudioClip audio_shoot_flare; //done
        public static AudioSource AudioSource_shootflare; 

        //personal assistent
        public static AudioClip audio_Quoted;
        public static AudioSource AudioSource_Quoted;
    }
}
