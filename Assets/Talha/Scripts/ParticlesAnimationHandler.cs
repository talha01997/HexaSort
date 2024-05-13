using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ParticlesAnimationHandler : MonoBehaviour
{
    public static ParticlesAnimationHandler instance;
    [SerializeField] GameObject myPrefab;
    [SerializeField] RectTransform starImg, canvas;
    [SerializeField] Transform starTarget;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AnimateParticles(Vector3 pos)
    {
        var starObj= Instantiate(myPrefab, pos, Quaternion.identity);
        starObj.transform.DOJump(Vector3.up, 1, 1, .5f);
        Destroy(starObj, .5f);
        starImg.position = WordPointToCanvasPoint(Camera.main, pos, canvas);
        starImg.gameObject.SetActive(true);
        starImg.DOMove(starTarget.position, .5f);
    }

    public Vector2 WordPointToCanvasPoint(Camera camera, Vector3 worldPoint, RectTransform canvasRect)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(worldPoint);
        Vector2 screenPosition = new Vector2(((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewportPosition.y * canvasRect.sizeDelta.y) -(canvasRect.sizeDelta.y * 0.5f)));
        return screenPosition;

    }
}
