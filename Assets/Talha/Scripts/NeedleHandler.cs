using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleHandler : MonoBehaviour
{
    [SerializeField] SpinWheel spinWheel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<WheelReward>())
        {
            spinWheel.coins = collision.gameObject.GetComponent<WheelReward>().coins;
        }
    }
}
