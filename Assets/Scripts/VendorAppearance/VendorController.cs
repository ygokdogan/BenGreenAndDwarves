using System;
using DG.Tweening;
using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace VendorAppearance
{
    public class VendorController : MonoBehaviour
    {
        public SpriteRenderer bodyRenderer, faceRenderer, mustacheRenderer, bagRenderer, hatRenderer, hairRenderer;

        public Transform enterPosition, exitPosition, offerPosition;
        public Animator anim;
        
        public void ApplyAppearance(VendorData data, VendorGenerator gen)
        {
            if (bodyRenderer) bodyRenderer.sprite = gen.GetBody(data.bodyIndex);
            if (faceRenderer) faceRenderer.sprite = gen.GetFace(data.faceIndex);
            if (mustacheRenderer) mustacheRenderer.sprite = gen.GetMustache(data.mustacheIndex);
            if (bagRenderer) bagRenderer.sprite = gen.GetBag(data.bagIndex);
            if (hatRenderer) hatRenderer.sprite = gen.GetHat(data.hatIndex);
            if (hairRenderer) hairRenderer.sprite = gen.GetHair(data.hairIndex);
        }

        public void AnimateEnter(Action onComplete = null)
        {
            transform.DOKill();
            transform.localPosition = enterPosition.localPosition;
            transform.DOLocalMove(offerPosition.localPosition, .65f)
                .SetEase(Ease.OutCubic);
        }

        public void AnimateExit(Action onComplete = null)
        {
            transform.DOKill();
            transform.DOLocalMove(exitPosition.localPosition, .65f)
                .SetEase(Ease.OutCubic);
        }
    }
}
