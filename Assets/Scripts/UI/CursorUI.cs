using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    /// <summary>
    /// Optional canvas-based cursor overlay.
    /// Attach this to a UI Image on a Screen Space - Overlay canvas (on top of everything).
    /// It will follow the mouse position, mirroring what CursorManager sets as the hardware cursor.
    ///
    /// Usage: Assign this to an Image GameObject in your UI hierarchy.
    /// Make sure Cursor.visible = false if you want to hide the OS cursor entirely.
    /// (CursorManager does NOT hide the OS cursor by default so you can choose.)
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CursorUI : MonoBehaviour
    {
        [Tooltip("If true, the OS cursor is hidden and this UI image is shown instead.")]
        public bool hideOsCursor = false;

        private RectTransform cursorTransform;
        private Canvas parentCanvas;
        private RectTransform canvasTransform;
        private Camera canvasCamera;

        private void Awake()
        {
            cursorTransform = GetComponent<RectTransform>();
            parentCanvas    = GetComponentInParent<Canvas>();

            if (parentCanvas)
            {
                canvasTransform = parentCanvas.GetComponent<RectTransform>();
                canvasCamera    = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : parentCanvas.worldCamera;
            }
        }

        private void OnEnable()
        {
            if (hideOsCursor) Cursor.visible = false;
        }

        private void OnDisable()
        {
            if (hideOsCursor) Cursor.visible = true;
        }

        private void Update()
        {
            if (cursorTransform == null || canvasTransform == null) return;

            Vector2 mousePos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : (Vector2)Input.mousePosition;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasTransform, mousePos, canvasCamera, out Vector2 localPoint))
            {
                cursorTransform.localPosition = localPoint;
            }
        }
    }
}
