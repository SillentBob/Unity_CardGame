using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform ParentToReturnTo
    {
        get => _parentToReturnTo;
        set => _parentToReturnTo = value;
    }

    private Transform _parentToReturnTo = null;
    private Vector3 _dragStartPosition;
    private bool _isDraggable = true;
    private GameObject _discardPile;

    private void Start()
    {
        _discardPile = GameObject.FindGameObjectWithTag("DiscardPile");
    }

    public void Setup([NotNull] CardModel model)
    {
        //this.cardModel = model;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDraggable) return;

        _parentToReturnTo = transform.parent;
        _dragStartPosition = transform.position;

        this.transform.SetParent(transform.root);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //TODO: add position interpolation from last frame to current frame pointer position instead of raw instant position change
        if (!_isDraggable)
        {
            return;
        }
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable) return;

        GetComponent<CanvasGroup>().blocksRaycasts = true;
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

        this.transform.SetParent(_parentToReturnTo);
        transform.position = _dragStartPosition;
    }

    public void SetDraggable(bool draggable)
    {
        _isDraggable = draggable;
    }

    public void DiscardCard()
    {
        transform.SetParent(_discardPile.transform);
        Destroy(this.gameObject);
    }
}