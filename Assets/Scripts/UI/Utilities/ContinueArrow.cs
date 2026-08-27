using System;
using DG.Tweening;
using UnityEngine;

namespace UI.Utilities
{
    public class ContinueArrow : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector2 startPos;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            startPos = rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            rectTransform.DOKill();
            rectTransform.anchoredPosition = startPos;
            rectTransform.DOAnchorPosY(startPos.y + 4f, 0.75f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
        }

        private void OnDisable()
        {
            rectTransform.DOKill();
            rectTransform.anchoredPosition = startPos;
        }
    }
}
