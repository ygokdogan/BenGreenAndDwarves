using ScriptableObjects;
using UnityEngine;

namespace VendorAppearance
{
    public class VendorSpawner : MonoBehaviour
    {
        public VendorGenerator generator;
        public VendorController controller;

        public GameObject vendorPrefab;

        public void SpawnNextVendor()
        {
            VendorData data = generator.Generate();

            VendorController v = Instantiate(vendorPrefab, transform.position, Quaternion.identity).GetComponent<VendorController>();
            v.ApplyAppearance(data, generator);
        }
    }
}