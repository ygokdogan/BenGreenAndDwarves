using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Vendor", menuName = "Data/Vendor")]
    public class VendorData : ScriptableObject
    {
        public string vendorName;
        public Sprite visual;
    }
}