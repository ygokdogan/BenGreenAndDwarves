using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UI.Utilities
{
    public class SwipeableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Drag & Swipe Settings")]
        [Tooltip("Distance in canvas units dragged to trigger selection action")]
        public float swipeThreshold = 150f;

        [Tooltip("Only trigger swipe action if dragged upwards beyond threshold")]
        public bool requireUpwardThreshold = true;

        [Tooltip("Allow dragging card in any 2D direction")]
        public bool allowFree2DDrag = true;

        [Tooltip("Speed at which card springs back to original position when released")]
        public float returnSpeed = 15f;
        
        [Tooltip("Maximum Z rotation angle while dragging")]
        public float maxTiltAngle = 25f;

        [Tooltip("Tilt sensitivity relative to horizontal drag speed")]
        public float tiltSensitivity = 0.6f;

        [Tooltip("Speed of tilt interpolation")]
        public float tiltLerpSpeed = 14f;

        [Header("Threshold Fade & Visual Settings")]
        [Tooltip("Fade in / Fade out animation speed for outline and visual highlights")]
        public float fadeSpeed = 16f;

        [Tooltip("Scale multiplier when card crosses threshold")]
        public float thresholdScale = 1.18f;

        [Tooltip("Optional graphic element to highlight when threshold is crossed")]
        public Graphic cardGraphic;

        [Tooltip("Color tint when crossing threshold (if cardGraphic is assigned)")]
        public Color thresholdColor = new Color(1f, 1f, 1f, 1f);

        [Header("Outline Fade Settings")]
        [Tooltip("Optional Outline component to highlight when threshold is crossed")]
        public Outline cardOutline;

        [Tooltip("Color of outline when threshold is crossed (Green for Accept, Red for Decline)")]
        public Color thresholdOutlineColor = Color.green;

        [Tooltip("Thickness/Distance of outline when threshold is crossed")]
        public Vector2 thresholdOutlineDistance = new Vector2(6f, -6f);

        [Tooltip("Automatically add an Outline component if missing on this GameObject")]
        public bool autoAddOutlineIfMissing = true;

        public AudioClip triggerSound;

        [Header("Events")]
        public UnityEvent onSwipedUp;
        public UnityEvent onHoverEnter;
        public UnityEvent onHoverExit;
        public UnityEvent<bool> onThresholdChanged;

        private RectTransform rectTransform;
        private Vector2 initialAnchoredPosition;
        private Vector3 initialScale;
        private Quaternion initialRotation;
        private int originalSiblingIndex;

        private bool isDragging = false;
        private bool isBeyondThreshold = false;
        private Coroutine returnCoroutine;
        private Canvas parentCanvas;

        private float targetTiltZ = 0f;
        private float currentTiltZ = 0f;

        private Color defaultGraphicColor;
        private Vector2 defaultOutlineDistance;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();

            if (rectTransform != null)
            {
                initialAnchoredPosition = rectTransform.anchoredPosition;
                initialScale = rectTransform.localScale;
                initialRotation = rectTransform.localRotation;
            }

            originalSiblingIndex = transform.GetSiblingIndex();

            if (cardGraphic == null)
            {
                cardGraphic = GetComponent<Graphic>();
            }

            if (cardGraphic != null)
            {
                defaultGraphicColor = cardGraphic.color;
            }

            if (cardOutline == null)
            {
                cardOutline = GetComponent<Outline>();
            }

            if (cardOutline == null && autoAddOutlineIfMissing)
            {
                cardOutline = gameObject.AddComponent<Outline>();
            }

            if (cardOutline != null)
            {
                defaultOutlineDistance = cardOutline.effectDistance;
                Color hiddenColor = thresholdOutlineColor;
                hiddenColor.a = 0f;
                cardOutline.effectColor = hiddenColor;
                cardOutline.enabled = false;
            }
        }

        private void OnEnable()
        {
            ResetPosition();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (isDragging)
            {
                currentTiltZ = Mathf.LerpAngle(currentTiltZ, targetTiltZ, dt * tiltLerpSpeed);
                if (rectTransform != null)
                {
                    rectTransform.localRotation = Quaternion.Euler(0, 0, currentTiltZ);
                }

                targetTiltZ = Mathf.Lerp(targetTiltZ, 0f, dt * 6f);
            }

            Vector3 targetScale = isBeyondThreshold ? initialScale * thresholdScale : initialScale;
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, dt * fadeSpeed);
            }

            if (cardGraphic != null)
            {
                Color targetColor = isBeyondThreshold ? thresholdColor : defaultGraphicColor;
                cardGraphic.color = Color.Lerp(cardGraphic.color, targetColor, dt * fadeSpeed);
            }

            if (cardOutline != null)
            {
                Color targetOutline = thresholdOutlineColor;
                if (!isBeyondThreshold)
                {
                    targetOutline.a = 0f;
                }

                if (isBeyondThreshold || cardOutline.effectColor.a > 0.01f)
                {
                    cardOutline.enabled = true;
                    cardOutline.effectColor = Color.Lerp(cardOutline.effectColor, targetOutline, dt * fadeSpeed);
                    cardOutline.effectDistance = Vector2.Lerp(cardOutline.effectDistance, isBeyondThreshold ? thresholdOutlineDistance : defaultOutlineDistance, dt * fadeSpeed);
                }
                else
                {
                    cardOutline.enabled = false;
                }
            }
        }

        public void ResetPosition()
        {
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }

            isDragging = false;
            isBeyondThreshold = false;
            targetTiltZ = 0f;
            currentTiltZ = 0f;

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = initialAnchoredPosition;
                rectTransform.localScale = initialScale;
                rectTransform.localRotation = initialRotation;
            }

            transform.SetSiblingIndex(originalSiblingIndex);

            if (cardGraphic != null)
            {
                cardGraphic.color = defaultGraphicColor;
            }

            if (cardOutline != null)
            {
                Color hiddenColor = thresholdOutlineColor;
                hiddenColor.a = 0f;
                cardOutline.effectColor = hiddenColor;
                cardOutline.effectDistance = defaultOutlineDistance;
                cardOutline.enabled = false;
            }
        }

        public void AnimateInFromBottom(float duration = 0.45f, float delay = 0f, float yOffset = 500f)
        {
            ResetPosition();
            if (rectTransform != null)
            {
                rectTransform.DOKill();
                rectTransform.anchoredPosition = initialAnchoredPosition - new Vector2(0, yOffset);
                rectTransform.DOAnchorPos(initialAnchoredPosition, duration)
                    .SetDelay(delay)
                    .SetEase(Ease.OutBack);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isDragging)
            {
                onHoverEnter?.Invoke();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isDragging)
            {
                onHoverExit?.Invoke();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            isBeyondThreshold = false;
            
            transform.SetAsLastSibling();

            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }

            onHoverEnter?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform == null) return;

            Vector2 delta = eventData.delta;

            if (parentCanvas != null && parentCanvas.scaleFactor > 0)
            {
                delta /= parentCanvas.scaleFactor;
            }

            Vector2 newPos = rectTransform.anchoredPosition + delta;

            if (!allowFree2DDrag)
            {
                newPos.x = initialAnchoredPosition.x;
                newPos.y = Mathf.Max(initialAnchoredPosition.y, newPos.y);
            }

            rectTransform.anchoredPosition = newPos;

            float horizontalOffset = newPos.x - initialAnchoredPosition.x;
            targetTiltZ = Mathf.Clamp(-delta.x * tiltSensitivity * 4f - horizontalOffset * 0.04f, -maxTiltAngle, maxTiltAngle);

            float draggedY = newPos.y - initialAnchoredPosition.y;
            float totalDistance = Vector2.Distance(newPos, initialAnchoredPosition);
            bool newlyBeyond = requireUpwardThreshold ? (draggedY >= swipeThreshold) : (totalDistance >= swipeThreshold);

            if (newlyBeyond != isBeyondThreshold)
            {
                isBeyondThreshold = newlyBeyond;
                onThresholdChanged?.Invoke(isBeyondThreshold);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            onHoverExit?.Invoke();

            if (isBeyondThreshold)
            {
                GameManager.Instance.uiButtonSource.PlayOneShot(triggerSound);
                onSwipedUp?.Invoke();
            }
            else
            {
                if (gameObject.activeInHierarchy)
                {
                    returnCoroutine = StartCoroutine(SmoothReturn());
                }
                else
                {
                    ResetPosition();
                }
            }
        }

        private IEnumerator SmoothReturn()
        {
            while (Vector2.Distance(rectTransform.anchoredPosition, initialAnchoredPosition) > 0.5f ||
                   Quaternion.Angle(rectTransform.localRotation, initialRotation) > 0.5f ||
                   Vector3.Distance(rectTransform.localScale, initialScale) > 0.01f ||
                   (cardOutline != null && cardOutline.effectColor.a > 0.01f))
            {
                float t = Time.deltaTime * returnSpeed;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, initialAnchoredPosition, t);
                rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, initialRotation, t);
                yield return null;
            }

            ResetPosition();
        }
    }
}
