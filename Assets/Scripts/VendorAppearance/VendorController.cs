using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace VendorAppearance
{
    public class VendorController : MonoBehaviour
    {
        public SpriteRenderer bodyRenderer, faceRenderer, mustacheRenderer, bagRenderer, hatRenderer, hairRenderer;
        public TMP_Text nameText;

        public void ApplyAppearance(VendorData data, VendorGenerator gen)
        {
            bodyRenderer.sprite = gen.GetBody(data.bodyIndex);
            //faceRenderer.sprite = gen.GetFace(data.faceIndex);
            //mustacheRenderer.sprite = gen.GetMustache(data.mustacheIndex);
            bagRenderer.sprite = gen.GetBag(data.bagIndex);
            //hatRenderer.sprite = gen.GetHat(data.hatIndex);
            //hairRenderer.sprite = gen.GetHair(data.hairIndex);
            nameText.text = data.vendorName;
        }
    }
}