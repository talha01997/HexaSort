using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [SerializeField] GameObject pausePanel;
    [SerializeField] Button hammerBtn, respawnBtn;
    [SerializeField] Transform sliderParent;
    [SerializeField] Slider lvlBarSlider;
    [SerializeField] TextMeshProUGUI scoreTxt;
    [SerializeField] int currentScore, totalScore;

    public event Action HammerOn, HammerOff;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    private void OnEnable()
    {
        //respawnBtn.onClick.AddListener(StackSpawner.instance.RespawnStack);
        //hammerBtn.onClick.AddListener(ActivateHammer);
    }

    private void OnDisable()
    {
        //respawnBtn.onClick.RemoveListener(StackSpawner.instance.RespawnStack);
        //hammerBtn.onClick.RemoveListener(ActivateHammer);
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {

        CanvasManager.instance.ScoreUpdatedEvent += UpdateScore;
        yield return new WaitForSeconds(.6f);
        SoundManager.instance.BGAudioSource.volume = 0;

        totalScore = GameManager.instance.MaxTargetScore;
        lvlBarSlider.maxValue = totalScore;
        lvlBarSlider.value = currentScore;
        scoreTxt.text = $"{currentScore}/{totalScore}";
    }

    void UpdateScore(int score)
    {
        currentScore += score;
        sliderParent.DOScale(1.2f, .25f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack);
        lvlBarSlider.DOValue(currentScore, .25f);
        scoreTxt.text = $"{currentScore}/{totalScore}";

        GridManager.instance.CheckLockedCells(currentScore);

    }
    public void OnClickPause()
    {
        SoundManager.instance.PlaySFXSound("Click");
        pausePanel.SetActive(true);
    }

    public void OnClickResume()
    {
        SoundManager.instance.PlaySFXSound("Click");
        pausePanel.SetActive(false);
    }

    public void OnClickHome()
    {
        SoundManager.instance.PlaySFXSound("Click");
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickRestart()
    {
        SoundManager.instance.PlaySFXSound("Click");
        SceneManager.LoadScene("PokerGameplay");
    }

    public void ActivateHammer()
    {
        SoundManager.instance.PlaySFXSound("Click");
        HammerOn?.Invoke();
    }

    public void DeActivateHammer()
    {
        HammerOff?.Invoke();
    }
}
