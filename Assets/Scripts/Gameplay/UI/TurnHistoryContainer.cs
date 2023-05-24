using UnityEngine;
using UnityEngine.EventSystems;


public class TurnHistoryContainer : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private float _maximumYOffset;
    private Vector3 _initialPosition;
    private float _initialMouseY;

    void Start()
    {
        _initialPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialMouseY = Input.mousePosition.y;
    }

    public void OnDrag(PointerEventData eventData)
    {

        float dragAmount = Input.mousePosition.y - _initialMouseY;
        float newContainerY = Mathf.Clamp(_initialPosition.y + dragAmount, _initialPosition.y, _initialPosition.y + _maximumYOffset);
        transform.position = new Vector3(transform.position.x, newContainerY, transform.position.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LeanTween.move(gameObject, _initialPosition, 0.3f).setEaseOutCirc();
        //transform.position = _initialPosition;
    }
}
