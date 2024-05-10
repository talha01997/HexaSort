using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsManager
{
    public static void SetLevel(int value)
    {
        PlayerPrefs.SetInt("Level", value);
    }

    public static int GetLevel(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }
}
