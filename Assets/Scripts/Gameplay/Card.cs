using System;
using Const;
using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;
    [SerializeField] private bool centerObjectToDragPressOrigin;

    public Transform ParentToReturnTo
    {
        get => _parentToReturnTo;
        set => _parentToReturnTo = value;
    }

    private Transform _parentToReturnTo;
    private Vector3 _dragStartPosition;
    private Vector2 _dragPressToDraggedObjectDelta;
    private bool _isDraggable = true;
    private GameObject _dragAnchor;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private int _canvasInitialSortingOrder;
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
        _canvasInitialSortingOrder = _canvas.sortingOrder;
    }

    public void Setup([NotNull] CardModel model, GameObject dragAnchor)
    {
        _dragAnchor = dragAnchor;
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
        _canvasGroup.blocksRaycasts = false;
        _canvas.sortingOrder = CanvasSortingOrder.ABOVE_DEFAULT;
        
        _parentToReturnTo = transform.parent;
        _dragStartPosition = transform.position;
        _dragPressToDraggedObjectDelta = (Vector2)_dragStartPosition - eventData.pressPosition;
        transform.SetParent(_dragAnchor.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
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
        if (dropZone != null)
        {
            if (dropZone.zoneType == DropArea.DropAreaType.PlayArea)
            {
                _isDraggable = false; //once played, do not allow undo
                PlayCard();
                return;
            }
        }
        transform.SetParent(_parentToReturnTo);
        transform.position = _dragStartPosition;
        _canvasGroup.blocksRaycasts = true;
        _canvas.sortingOrder = _canvasInitialSortingOrder;
    }

    private void PlayCard()
    {
        EventManager.Invoke(new PlayCardEvent(this));
    }
}