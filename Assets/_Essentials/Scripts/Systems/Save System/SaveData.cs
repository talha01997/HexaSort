//Shady
// Introduce your new variables under Game Variables and pass them accordingly
// in the Constructor => SaveData();
using UnityEngine;
using System;
public sealed class SaveData
{
    //===================================================
    // PRIVATE FIELDS
    //===================================================
    private static SaveData _instance = null;

    //===================================================
    // PROPERTIES
    //===================================================
    public static SaveData Instance
    {
        get
        {
            if(_instance is null)
            {
                _instance = new SaveData();
                SaveSystem.LoadProgress();
            }//if end
            return _instance;
        }//get end
    }//Property end

    //===================================================
    // FIELDS
    //===================================================
    
    public bool Music  = true;
    
    public bool SFX    = true;
    
    public bool Haptic = true;

    public bool firstPlay = true;
    [Space]
    
    public int Level  = 1;
    
    public int cash;

    public int avatarNum;

    public string userName;

    public DataToSave Data;

    [HideInInspector]
    public string HashOfSaveData = null;

    public SaveData(){}

    private SaveData(SaveData data)
    {
        Music  = data.Music;
        SFX    = data.SFX;
        Haptic = data.Haptic;
        firstPlay = data.firstPlay;
        Level  = data.Level;
        cash  = data.cash;
        avatarNum = data.avatarNum;
        userName = data.userName;
    }//CopyConstructor() end

    public SaveData CreateSaveObject() => new SaveData(_instance);

    public void Reset() => _instance = new SaveData();

}//class end

[Serializable]
public class DataToSave
{
    public string levelType;
    public int levelNum = 1;
}