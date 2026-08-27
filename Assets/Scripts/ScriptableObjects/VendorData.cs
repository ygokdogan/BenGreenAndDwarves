using UnityEngine;

namespace ScriptableObjects
{
    public struct VendorData
    {
        public string vendorName;
        public int bodyIndex, faceIndex, hairIndex, hatIndex, mustacheIndex, bagIndex;
        
        public override bool Equals(object obj) => 
            obj is VendorData o &&
            bodyIndex == o.bodyIndex && faceIndex == o.faceIndex && 
            hairIndex == o.hairIndex && hatIndex == o.hatIndex && 
            mustacheIndex == o.mustacheIndex && bagIndex == o.bagIndex;
        
        public override int GetHashCode() => (bodyIndex, faceIndex, hairIndex, hatIndex, mustacheIndex, bagIndex).GetHashCode();
    }
}