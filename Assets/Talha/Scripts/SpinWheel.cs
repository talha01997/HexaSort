using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class SpinWheel : MonoBehaviour
{
    public RectTransform spinWheel; // Assign the UI element or 2D object in the Inspector
    public float initialRotationAmount = 90f; // Initial amount of rotation
    public float rotationIncrement = 45f; // Incremental amount of rotation
    public float duration = 1f; // Duration of each rotation

    private float currentRotationAmount;
    public int coins;
    [SerializeField] Transform cashTarget;
    [SerializeField] List<GameObject> cashObjects;

    void Start()
    {
        currentRotationAmount = initialRotationAmount;
    }

    public void RotateWheel()
    {
        // Rotate the wheel by the current rotation amount
        spinWheel.DORotate(new Vector3(0, 0, -currentRotationAmount), duration, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => {
                     // Increment the rotation amount for the next spin
                     currentRotationAmount += rotationIncrement;
                     StartCoroutine(AnimateCash());
                     
                 });
    }

    void Rotate()
    {
        // Rotate the wheel by the current rotation amount
        spinWheel.DORotate(new Vector3(0, 0, -currentRotationAmount), duration, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => {
                     // Increment the rotation amount for the next spin
                     currentRotationAmount += rotationIncrement;
                     GiveCash();
                 });
    }


    void GiveCash()
    {
        print("coins:" + coins);
        EconomySystem.instance.AddCash(coins);
    }

    // Example method to trigger the rotation
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RotateWheel();
        }
    }

    public IEnumerator AnimateCash()
    {
        var _sequence = DOTween.Sequence();
        _sequence = DOTween.Sequence();

        foreach (var cash in cashObjects)
        {
            cash.SetActive(true);
        }
        //SoundManager.instance.PlaySFXSound("LevelUp");
        foreach (var cash in cashObjects)
        {
            //_sequence.Join(cash.transform.DOMove(cashCollectMidPoint.transform.position, UnityEngine.Random.Range(0.2f, 1f)));
            _sequence.Join(cash.transform.DOLocalJump(
                new Vector2(UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150)), 1, 1, 0.3f));
        }

        _sequence.OnComplete(
            () =>
            {
                foreach (var cash in cashObjects)
                {
                    cash.transform.DOMove(cashTarget.transform.position, UnityEngine.Random.Range(.15f, .65f))
                        .OnComplete(
                            () =>
                            {
                                cash.SetActive(false);
                                cash.transform.localPosition = Vector3.zero;
                            });
                }
                GiveCash();
            });
        yield return new WaitForSeconds(.5f);
    }
}