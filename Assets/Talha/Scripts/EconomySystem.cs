using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class EconomySystem : MonoBehaviour
{
    public static EconomySystem instance;
    [SerializeField] List<Text> cashTexts;
    [SerializeField] string currentCash;
    [SerializeField] int cash;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int GetCash()
    {
        return cash;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(.5f);
        if (File.Exists(SaveSystem.GetSavePath()))
        {
            LoadCashData();
        }
        else
        {
            SaveThisData();
            UpdateUI();
        }
    }
    /*[ContextMenu("TempCashAdd")]
    void TempCashAdd()
    {
        AddCash(cashToAdd);
        UpdateCash();
    }*/
    void LoadCashData()
    {
        SaveSystem.LoadProgress();
        //cash = SaveData.Instance.cash;
        currentCash = cash.ToString();
        UpdateUI();
    }

    void SaveThisData()
    {
        //SaveData.Instance.cash = cash;
        SaveSystem.SaveProgress();

    }
    public void AddCash(int amount)
    {
        cash += amount;
        SaveThisData();
        UpdateUI();
    }
    public void SubtractCash(int amount)
    {
        cash -= amount;
        SaveThisData();
        UpdateUI();
    }

    //public void AddStars(int amount)
    //{
    //    stars += amount;
    //    SaveThisData();
    //    UpdateUI();
    //}
    //public void SubtractStars(int amount)
    //{
    //    stars -= amount;
    //    SaveThisData();
    //    UpdateUI();
    //}

    void UpdateUI()
    {
        if (cashTexts.Count == 0)
            return;

        print("updated cash");
        foreach (var cashTxt in cashTexts)
        {
            cashTxt.text = GetCash().ToString();
        }
        //foreach (var starsTxt in starsTexts)
        //{
        //    starsTxt.text = GetStars().ToString();
        //}
    }
    void UpdateCash()
    {
        currentCash = cash.ToString();
    }

    /*private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            AddCash(500);
            UpdateCash();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SubtractCash(250);
            UpdateCash();
        }
    }*/
}