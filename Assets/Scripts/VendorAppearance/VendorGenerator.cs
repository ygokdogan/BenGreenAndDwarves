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
        private string[] firstNames, lastNames;
        
        HashSet<VendorData> used = new HashSet<VendorData>();

        private void Awake()
        {
            bodies = Resources.LoadAll<Sprite>("Appearance/VendorParts/Bodies");
            faces = Resources.LoadAll<Sprite>("Appearance/VendorParts/Faces");
            hairs = Resources.LoadAll<Sprite>("Appearance/VendorParts/Hairs");
            hats = Resources.LoadAll<Sprite>("Appearance/VendorParts/Hats");
            mustaches = Resources.LoadAll<Sprite>("Appearance/VendorParts/Mustaches");
            bags = Resources.LoadAll<Sprite>("Appearance/VendorParts/Bags");
            
            firstNames = LoadNames("Appearance/VendorParts/FirstNames");
            lastNames = LoadNames("Appearance/VendorParts/LastNames");
        }

        private string[] LoadNames(string path)
        {
            TextAsset file = Resources.Load<TextAsset>(path);
            if (file == null)
            {
                Debug.LogError($"Name file could not be found: {path}");
                return new[] { "Unknown" };
            }
            
            return file.text.Split('\n').Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToArray();
        }

        public VendorData Generate()
        {
            VendorData data;
            int safety = 0;
            do
            {
                data = new VendorData
                {
                    vendorName = RandomName(),
                    bodyIndex = Random.Range(0, bodies.Length),
                    faceIndex = Random.Range(0, faces.Length),
                    hairIndex = Random.Range(0, hairs.Length),
                    hatIndex = Random.Range(0, hats.Length),
                    mustacheIndex = Random.Range(0, mustaches.Length),
                    bagIndex = Random.Range(0, bags.Length),
                };
            } while (used.Contains(data) && safety < 100);
            
            used.Add(data);
            return data;
        }

        private string RandomName()
        {
            var first = firstNames[Random.Range(0, firstNames.Length)];
            var last = lastNames[Random.Range(0, lastNames.Length)];
            return $"{first} {last}";
        }

        public Sprite GetBody(int i) => bodies[i];
        public Sprite GetFace(int i) => faces[i];
        public Sprite GetHair(int i) => hairs[i];
        public Sprite GetHat(int i) => hats[i];
        public Sprite GetMustache(int i) => mustaches[i];
        public Sprite GetBag(int i) => bags[i];
    }
}