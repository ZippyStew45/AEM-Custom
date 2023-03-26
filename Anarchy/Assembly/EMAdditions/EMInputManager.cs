using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Anarchy.Configuration;
using UnityEngine;

public static class EMInputManager
{
    public enum EMInputs
    {
        Flare4,
        Flare5,
        Flare6,
        Flare7,
        DropWagon,
        ConnectWagon,
        HorseFollow,
        Interact,
        CommandList,
        Count
    }

    private static readonly KeyCode[] EMDefaults =
    {
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.F12,
        KeyCode.F
    };

    public static KeySetting[] Keys = new KeySetting[(int)EMInputs.Count];

    static EMInputManager()
    {
        for (int i = 0; i < (int)EMInputs.Count; i++)
        {
            Keys[i] = new KeySetting(((EMInputs)i).ToString(), EMDefaults[i]);
        }
    }

    public static bool IsInputDown(EMInputs input)
    {
        return Keys[(int)input].IsKeyDown();
    }

    public static string InputToString(EMInputs input)
    {
        return Keys[(int) input].Value.ToString();
    }
}
