using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Data",menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public static GameData instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log(instance);
        }
    }
    
    public void SaveMyData()
    {

    }
}
