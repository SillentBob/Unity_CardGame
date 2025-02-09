using System;
using System.Collections.Generic;
using System.Linq;
using Animators;
using Const;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Gameplay;
using JetBrains.Annotations;
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
    [SerializeField] private Animator cardAnimator;
    [SerializeField] private ParticleSystem trailParticles;

    public event Action<Card> OnPlayCard;
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
    
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
        _canvasInitialSortingOrder = _canvas.sortingOrder;
        cardFlipAnimator = new CardFlipAnimator(cardAnimator);
    }

    public void Setup([NotNull] CardModel model, GameObject dragAnchor)
    {
        _dragAnchor = dragAnchor;
        image.sprite = model.sprite;
        reverseImage.sprite = model.spriteReverse;
        SetCardHighlighted(false);
        EnableTrailParticles(false);
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
        
        if (Game.Instance.RenderMode == RenderMode.ScreenSpaceOverlay)
        {
            _dragPressToDraggedObjectDelta = (Vector2)_dragStartPosition - eventData.pressPosition;
        }
        else
        {
            _dragPressToDraggedObjectDelta = (Vector2)Camera.main.WorldToScreenPoint(_dragStartPosition) - eventData.pressPosition;
        }

        GraphicsRoot.transform.SetParent(_dragAnchor.transform);
        EnableTrailParticles(true, true).Forget();
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

        EnableTrailParticles(false);
        GraphicsRoot.transform.SetParent(transform);
        GraphicsRoot.transform.position = _dragStartPosition;
        _canvasGroup.blocksRaycasts = true;
        _canvas.sortingOrder = _canvasInitialSortingOrder;
        SetCardHighlighted(false);
        

        DropArea dropZone = null;
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (var result in raycastResults.Select(g => g.gameObject))
            // foreach (var result in eventData.hovered) this BS is not working
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
        // if (_isDraggable)
        // {
            SetCardHighlighted(true);
        // }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCardHighlighted(false);
    }

    private async UniTask EnableTrailParticles(bool value, bool delay = false)
    {
        if (delay)
        {
            await UniTask.DelayFrame(10);
        }

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
        cardFlipAnimator.PlayFlipToReverse();
    }

    public void PlayFlipFromReverseToForeground()
    {
        cardFlipAnimator.PlayFlipFromReverseToForeground();
    }
    
    private void RefreshHighlightOnPointerPosition()
    {
        List<RaycastResult> raycastResults = new();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        
        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        foreach (var result in raycastResults.Select(g => g.gameObject))
        {
            if ( result== this.gameObject || result.transform.IsChildOf(this.transform))
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
                SetCardHighlighted(true);
            }
        }
    }
}