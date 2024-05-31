using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    [SerializeField] GameObject menuPanel, settingsPanel, spinPanel, storePanel, profilePanel;
    [SerializeField] List<Image> menuBtns;
    [SerializeField] Color disabledColor;
    [SerializeField] int avatarNum;
    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.BGAudioSource.volume = .75f;
    }

    public void OnClickStore(int btnNum)
    {
        DisableMenuBtns();
        menuBtns[btnNum].color = Color.white;
        SoundManager.instance.PlaySFXSound("Click");
    }

    public void OnClickHome(int btnNum)
    {
        DisableMenuBtns();
        menuBtns[btnNum].color = Color.white;
        SoundManager.instance.PlaySFXSound("Click");
    }

    public void OnClickLeaderboard(int btnNum)
    {
        DisableMenuBtns();
        menuBtns[btnNum].color = Color.white;
        SoundManager.instance.PlaySFXSound("Click");
    }

    public void OnClickPlay()
    {
        SoundManager.instance.PlaySFXSound("Click");
        SceneManager.LoadScene("PokerGameplay");
    }

    public void OnClickSettings()
    {
        settingsPanel.SetActive(true);
        SoundManager.instance.PlaySFXSound("Click");
    }

    public void OnClickProfile()
    {
        profilePanel.SetActive(true);
        SoundManager.instance.PlaySFXSound("Click");
        //menuPanel.SetActive(false);
    }
    public void OnCloseProfile()
    {
        profilePanel.SetActive(false);
        SoundManager.instance.PlaySFXSound("Click");
        //menuPanel.SetActive(true);
    }
    public void OnClickSpin()
    {
        spinPanel.SetActive(true);
        SoundManager.instance.PlaySFXSound("Click");
    }

    void DisableMenuBtns()
    {
        foreach (var button in menuBtns)
        {
            button.color = disabledColor;
        }
    }
}