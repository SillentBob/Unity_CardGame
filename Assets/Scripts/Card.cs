using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;

    [SerializeField] [FormerlySerializedAs("centerObjectToDragOrigin")]
    private bool centerObjectToDragPressOrigin;

    public Transform ParentToReturnTo
    {
        get => _parentToReturnTo;
        set => _parentToReturnTo = value;
    }

    private Transform _parentToReturnTo = null;
    private Vector3 _dragStartPosition;
    private Vector2 _dragPressToDraggedObjectDelta;
    private bool _isDraggable = true;
    private GameObject _discardPile;
    private GameObject _dragAnchor;

    private void Start()
    {
        _discardPile = GameObject.FindGameObjectWithTag("DiscardPile");
        _dragAnchor = GameObject.FindGameObjectWithTag("DragAnchor");
    }

    public void Setup([NotNull] CardModel model)
    {
        image.sprite = model.sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag Card");
        if (!_isDraggable)
        {
            return;
        }

        _parentToReturnTo = transform.parent;
        _dragStartPosition = transform.position;
        _dragPressToDraggedObjectDelta = (Vector2)_dragStartPosition - eventData.pressPosition;
        transform.SetParent(_dragAnchor.transform);
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //TODO: add position interpolation from last frame to current frame pointer position instead of raw instant position change
        if (!_isDraggable)
        {
            return;
        }
        Debug.Log("OnDrag Card");
        transform.position = centerObjectToDragPressOrigin
            ? eventData.position
            : eventData.position + _dragPressToDraggedObjectDelta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable) return;

        //GetComponent<CanvasGroup>().blocksRaycasts = true;
        DropArea dropZone = null;

        foreach (var result in eventData.hovered)
        {
            dropZone = result.GetComponent<DropArea>();
            if (dropZone != null)
            {
                break;
            }
        }


        if (dropZone != null)
        {
            if (dropZone.zoneType == DropArea.DropAreaType.PlayArea)
            {
                dropZone.GetComponent<PlayArea>().PlayCard(this);
                return;
            }
            else if (dropZone.zoneType == DropArea.DropAreaType.DiscardPile)
            {
                DiscardCard();
                return;
            }
        }

        transform.SetParent(_parentToReturnTo);
        transform.position = _dragStartPosition;
    }

    public void SetDraggable(bool draggable)
    {
        _isDraggable = draggable;
    }

    private void DiscardCard()
    {
        transform.SetParent(_discardPile.transform);
        Destroy(gameObject);
    }
}