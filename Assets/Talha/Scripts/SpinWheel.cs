using UnityEngine;
using DG.Tweening;

public class SpinWheel : MonoBehaviour
{
    public RectTransform spinWheel; // Assign the UI element or 2D object in the Inspector
    public float initialRotationAmount = 90f; // Initial amount of rotation
    public float rotationIncrement = 45f; // Incremental amount of rotation
    public float duration = 1f; // Duration of each rotation

    private float currentRotationAmount;
    public int coins;
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
                     GiveCash();
                 });
    }

    void GiveCash()
    {
        print("coins:" + coins);
    }

    // Example method to trigger the rotation
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RotateWheel();
        }
    }

    
}