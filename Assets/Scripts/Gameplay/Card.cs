using System;
using System.Collections.Generic;
using System.Linq;
using Animators;
using Const;
using DefaultNamespace;
using Gameplay;
using JetBrains.Annotations;
using MethodExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private CardGraphicsRoot graphicsRoot;
    [SerializeField] private Image image;
    [SerializeField] private Image reverseImage;
    [SerializeField] private bool centerObjectToDragPressOrigin;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private ParticleSystem trailParticles;

    public event Action<Card> OnPlayCard;
    public event Action<Card, int, Card, int> OnSwapCardPlaces;
    public CardGraphicsRoot GraphicsRoot => graphicsRoot;
    public Transform ParentToReturnTo { get; set; }

    private Vector3 _dragStartPosition;
    private Vector2 _dragPressToDraggedObjectDelta;
    private bool _isDraggable = true;
    private GameObject _dragAnchor;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private int _canvasInitialSortingOrder;
    private CardFlipAnimator cardFlipAnimator;
    private bool _isPlayPossible;
    private int _originalIndex;

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
        reverseImage.sprite = model.spriteReverse;
        SetCardHighlighted(false);
        EnableTrailParticles(false);
        cardFlipAnimator = GetComponent<CardFlipAnimator>();
#if UNITY_EDITOR
        name = $"{name}_{model.name}";
#endif
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
        {
            return;
        }

        _canvasGroup.blocksRaycasts = false;
        _canvas.sortingOrder = CanvasSortingOrder.ABOVE_DEFAULT;

        ParentToReturnTo = transform;
        _dragStartPosition = GraphicsRoot.transform.position;
        _originalIndex = transform.GetSiblingIndex();

        if (Game.Instance.RenderMode == RenderMode.ScreenSpaceOverlay)
        {
            _dragPressToDraggedObjectDelta = (Vector2)_dragStartPosition - eventData.pressPosition;
        }
        else
        {
            _dragPressToDraggedObjectDelta =
                (Vector2)Camera.main.WorldToScreenPoint(_dragStartPosition) - eventData.pressPosition;
        }

        DetachGraphicsRootFromCard(_dragAnchor.transform, true);
        EnableTrailParticles(true);
        SetCardHighlighted(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
        {
            return;
        }

        var targetPos = centerObjectToDragPressOrigin
            ? eventData.position
            : eventData.position + _dragPressToDraggedObjectDelta;

        if (Game.Instance.RenderMode == RenderMode.ScreenSpaceOverlay)
        {
            GraphicsRoot.transform.position = targetPos;
        }
        else
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(targetPos);
            worldPoint.z = 0;
            GraphicsRoot.transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
        {
            return;
        }

        DropArea dropZone = null;
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        Card cardToSwap = null;
        foreach (var result in raycastResults.Select(g => g.gameObject))
        {
            dropZone = result.GetComponent<DropArea>();
            if (dropZone != null)
            {
                break;
            }

            if (result != this.gameObject)
            {
                var dropOnCard = result.GetComponent<Card>();
                if (dropOnCard != null)
                {
                    cardToSwap = dropOnCard;
                }
            }
        }

        EnableTrailParticles(false);
        _canvasGroup.blocksRaycasts = true;
        _canvas.sortingOrder = _canvasInitialSortingOrder;
        SetCardHighlighted(false);
        
        if (dropZone != null && dropZone.zoneType == DropArea.DropAreaType.PlayArea) //do not allow swapping cards in play area
        {
            SetDraggable(false); //once played, do not allow undo
            PlayCard();
        }
        else if (cardToSwap != null)
        {
            SwapCardsPlaces(_originalIndex, cardToSwap);
        }
        else
        {
            //not dropped on interactable object
            ReattachGraphicsRootToCard();
        }
    }

    private void SwapCardsPlaces(int originalIndex, Card dropOnCard)
    {
        var targetCardIdx = dropOnCard.transform.GetSiblingIndex();
        OnSwapCardPlaces?.Invoke(this, originalIndex, dropOnCard, targetCardIdx);
    }

    private void PlayCard()
    {
        if (OnPlayCard != null)
        {
            OnPlayCard.Invoke(this);
        }
    }

    private void SetCardHighlighted(bool value)
    {
        image.material = value ? highlightMaterial : normalMaterial;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isDraggable)
        {
            SetCardHighlighted(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCardHighlighted(false);
    }

    private void EnableTrailParticles(bool value)
    {
        if (trailParticles != null)
        {
            trailParticles.gameObject.SetActive(value);
            if (value)
            {
                trailParticles.Play();
            }
            else
            {
                trailParticles.Stop();
            }
        }
    }

    public void SetDraggable(bool value)
    {
        _isDraggable = value;
    }

    public void PlayFlipToReverse()
    {
        cardFlipAnimator?.PlayFlipToReverse();
    }

    public void PlayFlipFromReverseToForeground()
    {
        cardFlipAnimator?.PlayFlipFromReverseToForeground();
    }

    private void RefreshHighlightOnPointerPosition()
    {
        List<RaycastResult> raycastResults = new();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        foreach (var result in raycastResults.Select(g => g.gameObject))
        {
            if (result == this.gameObject || result.transform.IsChildOf(this.transform))
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
                SetCardHighlighted(true);
            }
        }
    }

    public void ReattachGraphicsRootToCard()
    {
        GraphicsRoot.transform.SetParent(transform);
        //recenter and stretch to its original values (reparenting changes sizedelta)
        GraphicsRoot.transform.localPosition = Vector3.zero;
        GraphicsRoot.gameObject.RectTransform().sizeDelta = Vector2.zero;
    }
    
    public void DetachGraphicsRootFromCard(Transform newParent, bool worldPositionStays)
    {
        GraphicsRoot.transform.SetParent(newParent, worldPositionStays);
    }
    
}