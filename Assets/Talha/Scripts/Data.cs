using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Data : MonoBehaviour
{
    public static Data instance;
    public string userName;
    public int avatarNum;
    public bool firstPlay;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(SaveSystem.GetSavePath()))
        {
            LoadSavedData();
        }
        else
        {
            MainMenuManager.instance.OnClickProfile();
            //SaveMyData();
            //EconomySystem.instance.AddCash(300);
        }
    }

    void LoadSavedData()
    {
        SaveSystem.LoadProgress();
        firstPlay = SaveData.Instance.firstPlay;
        userName = SaveData.Instance.userName;
        avatarNum = SaveData.Instance.avatarNum;
    }

    public void SaveMyData()
    {
        SaveData.Instance.firstPlay = firstPlay;
        SaveData.Instance.userName = userName;
        SaveData.Instance.avatarNum = avatarNum;
        SaveSystem.SaveProgress();
    }

}