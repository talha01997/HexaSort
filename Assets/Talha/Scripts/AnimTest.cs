using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class AnimTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Flip());
    }

    [ContextMenu("Flip")]
    IEnumerator Flip()
    {
        yield return new WaitForSeconds(1);
        transform.DOFlip();
        StartCoroutine(Flip());

    }
}
