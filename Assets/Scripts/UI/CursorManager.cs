using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public enum CursorState
    {
        Default,
        Hover,
        Hold,
        Click
    }
    
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }
        
        [Header("Cursor Sprites")]
        [Tooltip("Normal cursor shown when nothing is interactive.")]
        public Sprite defaultCursor;

        [Tooltip("Cursor shown when hovering over a swipeable card.")]
        public Sprite hoverCursor;

        [Tooltip("Cursor shown while pressing and holding (or dragging) a card.")]
        public Sprite holdCursor;

        [Tooltip("Cursor shown briefly on click (tap without hold).")]
        public Sprite clickCursor;
        
        [Header("Hotspots (pixel offset from top-left of cursor texture)")]
        public Vector2 defaultHotspot = Vector2.zero;
        public Vector2 hoverHotspot   = Vector2.zero;
        public Vector2 holdHotspot    = Vector2.zero;
        public Vector2 clickHotspot   = Vector2.zero;

        [Header("Resolution Scaling")]
        [Tooltip("The resolution at which the cursor sprites are displayed at their original size.")]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);

        [Tooltip("Limits applied to the resolution-based cursor scale.")]
        [SerializeField, Min(0.01f)] private float minimumScale = 0.5f;
        [SerializeField, Min(0.01f)] private float maximumScale = 3f;
        
        [Header("Click Cursor")]
        [Tooltip("How long the Click cursor stays visible before reverting.")]
        public float clickDuration = 0.12f;
        
        private bool _isHolding  = false;
        private int  _hoverCount = 0;
        private Coroutine _clickCoroutine;
        private readonly Dictionary<Sprite, Texture2D> _scaledCursorTextures = new();
        private CursorState _currentState = CursorState.Default;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        
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

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            ApplyState(CursorState.Default);
        }

        private void Update()
        {
            if (_lastScreenWidth == Screen.width && _lastScreenHeight == Screen.height)
                return;

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            ClearScaledCursorTextures();
            ApplyState(_currentState);
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
            ClearScaledCursorTextures();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #endregion
        
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
            _currentState = state;

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

            float scale = GetCursorScale();
            Texture2D cursorTexture = GetScaledCursorTexture(sprite, scale);
            Vector2 scaledHotspot = hotspot * scale;

            if (cursorTexture != null)
            {
                scaledHotspot.x = Mathf.Clamp(scaledHotspot.x, 0f, cursorTexture.width - 1);
                scaledHotspot.y = Mathf.Clamp(scaledHotspot.y, 0f, cursorTexture.height - 1);
            }

            Cursor.SetCursor(cursorTexture, scaledHotspot, CursorMode.ForceSoftware);
        }

        private float GetCursorScale()
        {
            float referenceWidth = Mathf.Max(1f, referenceResolution.x);
            float referenceHeight = Mathf.Max(1f, referenceResolution.y);
            float resolutionScale = Mathf.Min(Screen.width / referenceWidth, Screen.height / referenceHeight);
            float minScale = Mathf.Min(minimumScale, maximumScale);
            float maxScale = Mathf.Max(minimumScale, maximumScale);
            return Mathf.Clamp(resolutionScale, minScale, maxScale);
        }

        private Texture2D GetScaledCursorTexture(Sprite sprite, float scale)
        {
            if (sprite == null || sprite.texture == null)
                return null;

            if (Mathf.Approximately(scale, 1f))
                return sprite.texture;

            if (_scaledCursorTextures.TryGetValue(sprite, out Texture2D cachedTexture))
                return cachedTexture;

            Texture2D sourceTexture = sprite.texture;
            int width = Mathf.Max(1, Mathf.RoundToInt(sourceTexture.width * scale));
            int height = Mathf.Max(1, Mathf.RoundToInt(sourceTexture.height * scale));
            RenderTexture previousTarget = RenderTexture.active;
            RenderTexture temporaryTarget = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

            Graphics.Blit(sourceTexture, temporaryTarget);
            RenderTexture.active = temporaryTarget;

            Texture2D scaledTexture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = $"{sourceTexture.name} (Cursor Scale {scale:0.##})",
                filterMode = FilterMode.Point
            };
            scaledTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            scaledTexture.Apply();

            RenderTexture.active = previousTarget;
            RenderTexture.ReleaseTemporary(temporaryTarget);
            _scaledCursorTextures.Add(sprite, scaledTexture);
            return scaledTexture;
        }

        private void ClearScaledCursorTextures()
        {
            foreach (Texture2D texture in _scaledCursorTextures.Values)
                Destroy(texture);

            _scaledCursorTextures.Clear();
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
