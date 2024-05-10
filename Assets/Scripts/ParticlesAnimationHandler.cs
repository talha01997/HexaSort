using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAnimationHandler : MonoBehaviour
{
    [SerializeField] GameObject myPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AnimateParticles(Vector3 pos)
    {
        Instantiate(myPrefab, pos, Quaternion.identity);
    }

    public Vector2 WordPointToCanvasPoint(Camera camera, Vector3 worldPoint, RectTransform canvasRect)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(worldPoint);
        Vector2 screenPosition = new Vector2(((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewportPosition.y * canvasRect.sizeDelta.y) -(canvasRect.sizeDelta.y * 0.5f)));
        return screenPosition;

    }
}
