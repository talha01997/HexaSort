using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Data : MonoBehaviour
{
    public static Data instance;
    public int selectedMode;
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
            SaveMyData();
            EconomySystem.instance.AddCash(300);
        }
    }

    void LoadSavedData()
    {
        SaveSystem.LoadProgress();

    }

    public void SaveMyData()
    {

        SaveSystem.SaveProgress();
    }

}