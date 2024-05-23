using UnityEngine;
using UnityEngine.EventSystems;

public class HexParentHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Vector3 startPos, mouseOffset;
    [SerializeField] bool isPicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPicked = false;
    }

    private void OnEnable()
    {
        startPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isPicked)
        {
            Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

            // Update the position of the selected object to the mouse position
            transform.position = new Vector3(mousePosition.x + mouseOffset.x, transform.position.y, mousePosition.z + mouseOffset.z);
        }
        else
        {
            transform.localPosition = startPos;
        }
    }
}