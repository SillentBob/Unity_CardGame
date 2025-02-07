using System.Collections.Generic;
using System.Linq;
using System.Text;
using Const;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;

    [SerializeField] private bool centerObjectToDragPressOrigin;

    public Transform ParentToReturnTo
    {
        get => _parentToReturnTo;
        set => _parentToReturnTo = value;
    }

    private Transform _parentToReturnTo = null;
    private Vector3 _dragStartPosition;
    private Vector2 _dragPressToDraggedObjectDelta;
    private bool _isDraggable = true;
    private DiscardPile _discardPile;
    private GameObject _dragAnchor;

    private void Start()
    {
        _discardPile = GameObject.FindGameObjectWithTag(GameObjectTags.DISCARD_PILE).GetComponent<DiscardPile>();
        _dragAnchor = GameObject.FindGameObjectWithTag(GameObjectTags.DRAG_ANCHOR);
    }

    public void Setup([NotNull] CardModel model)
    {
        image.sprite = model.sprite;
#if UNITY_EDITOR
       name = $"{name}_{model.name}";
#endif
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag Card {this}");
        if (!_isDraggable)
        {
            return;
        }

        _parentToReturnTo = transform.parent;
        _dragStartPosition = transform.position;
        _dragPressToDraggedObjectDelta = (Vector2)_dragStartPosition - eventData.pressPosition;
        transform.SetParent(_dragAnchor.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //TODO: add position interpolation from last frame to current frame pointer position instead of raw instant position change
        if (!_isDraggable)
        {
            return;
        }
        
        transform.position = centerObjectToDragPressOrigin
            ? eventData.position
            : eventData.position + _dragPressToDraggedObjectDelta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
        {
            return;
        }

        DropArea dropZone = null;

        foreach (var result in eventData.hovered)
        {
            dropZone = result.GetComponent<DropArea>();
            if (dropZone != null)
            {
                break;
            }
        }
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Debug.Log($"OnEndDrag dropZone.zoneType: {dropZone?.zoneType}");
        
        if (dropZone != null)
        {
            if (dropZone.zoneType == DropArea.DropAreaType.PlayArea)
            {
                SetDraggable(false); //once played, do not allow undo
                dropZone.GetComponent<PlayArea>().PlayCard(this);
                return;
            }
            else if
                (dropZone.zoneType ==
                 DropArea.DropAreaType.DiscardPile) // optional destroy unwanted card to draw more next turn
            {
                DiscardCard();
                return;
            }
        }

        transform.SetParent(_parentToReturnTo);
        transform.position = _dragStartPosition;
    }

    private void SetDraggable(bool draggable)
    {
        _isDraggable = draggable;
    }

    private void DiscardCard()
    {
        _discardPile.AddCardToDiscardPile(this);
    }
}