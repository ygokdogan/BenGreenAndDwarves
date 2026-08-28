using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VendorAppearance
{
    public class VendorGenerator : MonoBehaviour
    {
        private Sprite[] bodies, faces, hairs, hats, mustaches, bags;
        
        HashSet<VendorData> used = new HashSet<VendorData>();

        private void Awake()
        {
            bodies = Resources.LoadAll<Sprite>("Appearance/VendorParts/Bodies");
            faces = Resources.LoadAll<Sprite>("Appearance/VendorParts/Faces");
            hairs = Resources.LoadAll<Sprite>("Appearance/VendorParts/Hairs");
            hats = Resources.LoadAll<Sprite>("Appearance/VendorParts/Hats");
            mustaches = Resources.LoadAll<Sprite>("Appearance/VendorParts/Mustaches");
            bags = Resources.LoadAll<Sprite>("Appearance/VendorParts/Bags");
        }

        public VendorData Generate()
        {
            VendorData data;
            int safety = 0;
            do
            {
                data = new VendorData
                {
                    bodyIndex = GetRandomIndex(bodies),
                    faceIndex = GetRandomIndex(faces),
                    hairIndex = GetRandomIndex(hairs),
                    hatIndex = GetRandomIndex(hats),
                    mustacheIndex = GetRandomIndex(mustaches),
                    bagIndex = GetRandomIndex(bags),
                };
                safety++;
            } while (used.Contains(data) && safety < 100);
            
            used.Add(data);
            return data;
        }

        private int GetRandomIndex(Sprite[] array)
        {
            if (array == null || array.Length == 0) return -1;
            return Random.Range(0, array.Length);
        }
        
        public Sprite GetBody(int i) => GetSpriteSafe(bodies, i);
        public Sprite GetFace(int i) => GetSpriteSafe(faces, i);
        public Sprite GetHair(int i) => GetSpriteSafe(hairs, i);
        public Sprite GetHat(int i) => GetSpriteSafe(hats, i);
        public Sprite GetMustache(int i) => GetSpriteSafe(mustaches, i);
        public Sprite GetBag(int i) => GetSpriteSafe(bags, i);

        private Sprite GetSpriteSafe(Sprite[] array, int index)
        {
            if (array == null || index < 0 || index >= array.Length) return null;
            return array[index];
        }
    }
}