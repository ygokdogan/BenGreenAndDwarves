using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StatBar
{
    public class StatBar : MonoBehaviour
    {
        private int maxValue = 100;

        private RectTransform bar;

        private Color32 healColor = new Color32(0, 255, 102, 255);
        private Color damageColor = new Color32(255, 0, 60, 255);
        public TextMeshProUGUI valuePopup;
        public TextMeshProUGUI value;
        private float popupY;
        
        private Slider fill;
        public Slider chunk;
        public Image flash;
        public Image outlineHighlight;
        public Image dangerHighlight;

        private const int DangerLow  = 15;
        private const int DangerHigh = 85;

        private static readonly Color32 DangerColor = new Color32(255, 40, 40, 255);
        private bool _inDanger;

        private void Awake()
        {
            bar = GetComponent<RectTransform>();
            fill = GetComponent<Slider>();
            popupY = valuePopup.rectTransform.anchoredPosition.y;

            if (outlineHighlight)
            {
                Color c = outlineHighlight.color;
                c.a = 0f;
                outlineHighlight.color = c;
                outlineHighlight.gameObject.SetActive(false);
            }

            if (dangerHighlight)
            {
                Color c = dangerHighlight.color;
                c.a = 0f;
                dangerHighlight.color = c;
                dangerHighlight.gameObject.SetActive(false);
            }
        }

        public void SetValue(float newValue, float oldValue)
        {
            HideHighlight();
            UpdateDangerZone((int)newValue);

            float val = oldValue;
            var diff = newValue - oldValue;

            if (diff == 0) return;
            
            float v = newValue / maxValue;

            fill.DOKill();
            chunk.DOKill();

            //DAMAGE
            if (diff < 0)
            {
                fill.DOValue(v, .6f).SetEase(Ease.OutCubic);
                chunk.DOValue(v - 0.035f, .7f).SetDelay(.8f).SetEase(Ease.OutCubic);
                
                bar.DOShakeAnchorPos(.1f, new Vector2(12, 5), 20);
                bar.DOPunchScale(new Vector3(0.03f, .25f, 0f), .3f);
                
                flash.gameObject.SetActive(true);
                flash.DOFade(0.85f, 0.04f).SetLoops(2, LoopType.Yoyo).OnComplete(() => flash.gameObject.SetActive(false));

                DOTween.To(() => val, x =>
                {
                    val = x;
                    value.text = Mathf.RoundToInt(val).ToString() + $"/{maxValue}";
                }, newValue, .6f);
            }
            else // HEAL
            {
                chunk.DOValue(v - 0.035f, .3f).SetEase(Ease.OutQuad);
                fill.DOValue(v, .6f).SetDelay(.6f).SetEase(Ease.OutQuad);
                
                bar.DOPunchScale(new Vector3(0.02f, .1f, 0f), .3f);
                DOTween.To(() => val, x =>
                {
                    val = x;
                    value.text = Mathf.RoundToInt(val).ToString() + $"/{maxValue}";
                }, newValue, 1.2f);
            }
            
            SetValuePopup((int)diff);
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                ShowHighlight();
            else
                HideHighlight();
        }

        private void ShowHighlight()
        {
            if (!outlineHighlight) return;

            outlineHighlight.gameObject.SetActive(true);
            outlineHighlight.DOKill();
            
            Color c = outlineHighlight.color;
            c.a = 1f;
            outlineHighlight.color = c;
            
            outlineHighlight.DOFade(0.7f, 0.7f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void HideHighlight()
        {
            if (!outlineHighlight) return;

            outlineHighlight.DOKill();
            
            outlineHighlight.DOFade(0f, 0.2f).OnComplete(() => 
            {
                outlineHighlight.gameObject.SetActive(false);
            });
        }

        // ── Danger Zone ──────────────────────────────────────────────

        private void UpdateDangerZone(int value)
        {
            bool danger = value <= DangerLow || value >= DangerHigh;

            if (danger == _inDanger) return;
            _inDanger = danger;

            if (danger)
                ShowDangerHighlight();
            else
                HideDangerHighlight();
        }

        private void ShowDangerHighlight()
        {
            if (!dangerHighlight) return;

            dangerHighlight.color = DangerColor;
            dangerHighlight.gameObject.SetActive(true);
            dangerHighlight.DOKill();

            // Fast aggressive pulse: 0.85 → 0.15 alpha, looping
            dangerHighlight.DOFade(0.15f, 0.4f)
                .From(0.85f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void HideDangerHighlight()
        {
            if (!dangerHighlight) return;

            dangerHighlight.DOKill();
            dangerHighlight.DOFade(0f, 0.25f).OnComplete(() =>
            {
                dangerHighlight.gameObject.SetActive(false);
            });
        }

        private void SetValuePopup(int value)
        {
            valuePopup.text = value.ToString();
            valuePopup.gameObject.SetActive(true);

            valuePopup.color = value > 0 ? healColor : damageColor;
            Color c = valuePopup.color;
            c.a = 1f;
            valuePopup.color = c;
            
            valuePopup.rectTransform.DOAnchorPosY(popupY + 20f, 1.85f).SetEase(Ease.OutCubic).OnComplete(ResetPopup);
            valuePopup.DOFade(0f, 0.35f).SetDelay(1.5f);
        }

        private void ResetPopup()
        {
            valuePopup.text = "";
            valuePopup.gameObject.SetActive(false);
            valuePopup.rectTransform.anchoredPosition = new Vector2(valuePopup.rectTransform.anchoredPosition.x, popupY);
        }
    }
}