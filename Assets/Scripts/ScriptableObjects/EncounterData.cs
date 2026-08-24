using Stats;
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class EncounterData : ScriptableObject
    {
        public VendorData vendor;
        [TextArea] public string saleText;

        public int revealDelay = 0;
        [Space]
        public StatEffect[] rejectedEffects;

        public abstract StatEffect[] GetActualEffects();
    }
}