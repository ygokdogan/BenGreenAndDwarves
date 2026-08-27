using System;
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
        private TextMeshProUGUI valuePopup;
        private float popupY;
        
        private Slider fill;
        public Slider chunk;
        public Image flash;
        public Image outlineHighlight;

        private void Awake()
        {
            bar = GetComponent<RectTransform>();
            fill = GetComponent<Slider>();
            valuePopup = GetComponentInChildren<TextMeshProUGUI>();
            popupY = valuePopup.rectTransform.anchoredPosition.y;

            if (outlineHighlight)
            {
                Color c = outlineHighlight.color;
                c.a = 0f;
                outlineHighlight.color = c;
                outlineHighlight.gameObject.SetActive(false);
            }
        }

        public void SetValue(float newValue, float oldValue)
        {
            HideHighlight();
            
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
                
                flash.DOFade(0.85f, 0.04f).SetLoops(2, LoopType.Yoyo);
            }
            else // HEAL
            {
                chunk.DOValue(v - 0.035f, .3f).SetEase(Ease.OutQuad);
                fill.DOValue(v, .6f).SetDelay(.6f).SetEase(Ease.OutQuad);
                
                bar.DOPunchScale(new Vector3(0.02f, .1f, 0f), .3f);
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