using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// Cursor state enum.
    /// Priority (highest first): Hold > Click > Hover > Default
    /// </summary>
    public enum CursorState
    {
        Default,
        Hover,
        Hold,   // Pressed + held (includes drag)
        Click
    }

    /// <summary>
    /// Singleton that manages the custom hardware cursor.
    ///
    /// Setup:
    ///   1. Assign a Sprite for each cursor state in the Inspector.
    ///   2. Optionally adjust the hotspot per cursor state.
    ///   3. Place this component on a persistent GameObject.
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        // ── Cursor Sprites ────────────────────────────────────────────────────
        [Header("Cursor Sprites")]
        [Tooltip("Normal cursor shown when nothing is interactive.")]
        public Sprite defaultCursor;

        [Tooltip("Cursor shown when hovering over a swipeable card.")]
        public Sprite hoverCursor;

        [Tooltip("Cursor shown while pressing and holding (or dragging) a card.")]
        public Sprite holdCursor;

        [Tooltip("Cursor shown briefly on click (tap without hold).")]
        public Sprite clickCursor;

        // ── Hotspots ──────────────────────────────────────────────────────────
        [Header("Hotspots (pixel offset from top-left of cursor texture)")]
        public Vector2 defaultHotspot = Vector2.zero;
        public Vector2 hoverHotspot   = Vector2.zero;
        public Vector2 holdHotspot    = Vector2.zero;
        public Vector2 clickHotspot   = Vector2.zero;

        // ── Click flash ───────────────────────────────────────────────────────
        [Header("Click Cursor")]
        [Tooltip("How long the Click cursor stays visible before reverting.")]
        public float clickDuration = 0.12f;

        // ── Runtime state ─────────────────────────────────────────────────────
        private bool _isHolding  = false;
        private int  _hoverCount = 0;
        private Coroutine _clickCoroutine;

        // ─────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ApplyState(CursorState.Default);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Clear any stale hover/hold state left over from the previous scene.
            // UI objects are destroyed on load so OnPointerExit never fires for them.
            ResetAll();
        }

        private void OnDestroy()
        {
            if (Instance != this) return; // duplicate being destroyed — don't touch the cursor
            Instance = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Public API

        /// <summary>Pointer entered an interactive element.</summary>
        public void OnHoverEnter()
        {
            _hoverCount++;
            RefreshState();
        }

        /// <summary>Pointer exited an interactive element.</summary>
        public void OnHoverExit()
        {
            _hoverCount = Mathf.Max(0, _hoverCount - 1);
            RefreshState();
        }

        /// <summary>Pointer pressed down on a card (no drag yet).</summary>
        public void OnPressDown()
        {
            _isHolding = true;
            RefreshState();
        }

        /// <summary>
        /// Pointer released after a simple click (no drag occurred).
        /// stillHovering should be true because the pointer is still over the card.
        /// </summary>
        public void OnPressUp()
        {
            _isHolding = false;
            RefreshState();
        }

        /// <summary>
        /// Drag ended. Pass whether the pointer is still over the card rect.
        /// During drag Unity suppresses OnPointerExit, so we fix the hover count here.
        /// </summary>
        public void OnDragEnd(bool stillOverCard)
        {
            _isHolding = false;

            // During drag, OnPointerExit was suppressed by Unity.
            // If pointer is no longer over the card, manually compensate.
            if (!stillOverCard)
                _hoverCount = Mathf.Max(0, _hoverCount - 1);

            RefreshState();
        }

        /// <summary>Flash the Click cursor briefly, then revert.</summary>
        public void OnClick()
        {
            if (clickCursor == null) return;
            if (_clickCoroutine != null) StopCoroutine(_clickCoroutine);
            _clickCoroutine = StartCoroutine(ClickFlash());
        }

        /// <summary>Reset all state and revert to Default cursor.</summary>
        public void ResetAll()
        {
            if (_clickCoroutine != null) { StopCoroutine(_clickCoroutine); _clickCoroutine = null; }
            _isHolding  = false;
            _hoverCount = 0;
            ApplyState(CursorState.Default);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Private Helpers

        private void RefreshState()
        {
            if (_clickCoroutine != null) return;

            if (_isHolding)
                ApplyState(CursorState.Hold);
            else if (_hoverCount > 0)
                ApplyState(CursorState.Hover);
            else
                ApplyState(CursorState.Default);
        }

        private void ApplyState(CursorState state)
        {
            Sprite sprite = state switch
            {
                CursorState.Hover  => hoverCursor != null ? hoverCursor : defaultCursor,
                CursorState.Hold   => holdCursor  != null ? holdCursor  : defaultCursor,
                CursorState.Click  => clickCursor != null ? clickCursor : defaultCursor,
                _                  => defaultCursor
            };

            Vector2 hotspot = state switch
            {
                CursorState.Hover  => hoverHotspot,
                CursorState.Hold   => holdHotspot,
                CursorState.Click  => clickHotspot,
                _                  => defaultHotspot
            };

            Cursor.SetCursor(sprite != null ? sprite.texture : null, hotspot, CursorMode.ForceSoftware);
        }

        private IEnumerator ClickFlash()
        {
            ApplyState(CursorState.Click);
            yield return new WaitForSecondsRealtime(clickDuration);
            _clickCoroutine = null;
            RefreshState();
        }

        #endregion
    }
}
