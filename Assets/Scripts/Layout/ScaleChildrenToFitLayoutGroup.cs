using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScaleChildrenToFitLayoutGroup : MonoBehaviour
{
    private HorizontalOrVerticalLayoutGroup _layoutGroup;
    private RectTransform _parentRect;

    private int _lastChildCount;

    void Start()
    {
        _layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
        _parentRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            return;
        }
        if (_lastChildCount != childCount)
        {
            TryScaleChildren();
            _lastChildCount = childCount;
        }
    }
    
    private void TryScaleChildren()
    {
        if (_layoutGroup == null || _parentRect == null)
        {
            return;
        }

        float childrenHeightMax = 0;
        float totalChildrenWidth = 0;
        foreach (RectTransform child in transform)
        {
            totalChildrenWidth += child.rect.width;
            if (child.rect.height > childrenHeightMax)
            {
                childrenHeightMax = child.rect.height;
            }
        }

        float availableWidth = _parentRect.rect.width - _layoutGroup.transform.childCount*_layoutGroup.spacing - 
                               _layoutGroup.padding.left - _layoutGroup.padding.right ;

        if (totalChildrenWidth > availableWidth)
        {
            float scaleFactorX = availableWidth / totalChildrenWidth;

            foreach (RectTransform child in transform)
            {
                child.localScale = new Vector3(scaleFactorX, scaleFactorX, 1);
            }
        }
        else // Reset scale if children fit:
        {
            foreach (RectTransform child in transform)
            {
                child.localScale = Vector3.one;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_parentRect);
    }
}