using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Unity.VisualScripting;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
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
        totalScore = GameManager.instance.MaxTargetScore;
        lvlBarSlider.maxValue = totalScore;
        lvlBarSlider.value = currentScore;
        scoreTxt.text = $"{currentScore}/{totalScore}";
    }

    void UpdateScore(int score)
    {
        currentScore += 10;
        sliderParent.DOScale(1.2f, .25f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack);
        lvlBarSlider.DOValue(currentScore, .25f);
        scoreTxt.text = $"{currentScore}/{totalScore}";

        GridManager.instance.CheckLockedCells(currentScore);

    }

    public void ActivateHammer()
    {
        HammerOn?.Invoke();
    }

    public void DeActivateHammer()
    {
        HammerOff?.Invoke();
    }
}
