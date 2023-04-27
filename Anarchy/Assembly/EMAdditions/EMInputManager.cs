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
        Flare8,
        DropWagon,
        ConnectWagon,
        HorseFollow,
        Interact,
        CommandList,
        Builder_Place,
        Builder_delete,
        Builder_OBJ_away,
        Builder_OBJ_close,
        Supply_Drop_Gas,
        Supply_Drop_Blade,
        Count
    }

    private static readonly KeyCode[] EMDefaults =
    {
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.F,
        KeyCode.F12,
        KeyCode.Mouse0,
        KeyCode.Mouse1,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.J,
        KeyCode.K
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
