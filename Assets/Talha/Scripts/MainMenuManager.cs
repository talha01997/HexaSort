using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject menuPanel, settingsPanel, spinPanel, storePanel, profilePanel;

    [SerializeField] List<Image> menuBtns;
    [SerializeField] Color disabledColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickStore(int btnNum)
    {
        DisableMenuBtns();
        menuBtns[btnNum].color = Color.white;
    }

    public void OnClickHome(int btnNum)
    {
        DisableMenuBtns();
        menuBtns[btnNum].color = Color.white;
    }

    public void OnClickLeaderboard(int btnNum)
    {
        DisableMenuBtns();
        menuBtns[btnNum].color = Color.white;
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene("PokerGameplay");
    }

    public void OnClickSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OnClickProfile()
    {
        profilePanel.SetActive(true);
    }

    public void OnClickSpin()
    {
        spinPanel.SetActive(true);
    }

    void DisableMenuBtns()
    {
        foreach (var button in menuBtns)
        {
            button.color = disabledColor;
        }
    }
}