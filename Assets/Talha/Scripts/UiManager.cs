using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [SerializeField] Transform sliderParent;
    [SerializeField] Slider lvlBarSlider;
    [SerializeField] TextMeshProUGUI scoreTxt;
    [SerializeField] int currentScore, totalScore;

    private void Awake()
    {
        if (!instance)
            instance = this;
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
}
