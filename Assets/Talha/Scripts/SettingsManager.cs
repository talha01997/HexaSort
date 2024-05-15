using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] List<ToggleButtons> toggleButtons;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in toggleButtons)
        {
            item.Init();
        }
    }
}

[Serializable]
public class ToggleButtons
{
    public string btnName;
    public Button toggleBtn;
    public Sprite offSprite, onSprite;
    public bool state = true;

    public void Init()
    {
        toggleBtn.onClick.AddListener(OnButtonToggle);
    }

    public void OnButtonToggle()
    {
        switch (state)
        {
            case true:
                toggleBtn.transform.DOLocalMoveX(45, .2f);
                toggleBtn.GetComponent<Image>().sprite = offSprite;
                state = false;
                break;
            case false:
                toggleBtn.transform.DOLocalMoveX(-43.6f, .2f);
                toggleBtn.GetComponent<Image>().sprite = onSprite;
                state = true;
                break;

            default:
                break;
        }
    }
}