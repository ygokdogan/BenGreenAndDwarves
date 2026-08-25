using Stats;
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class EncounterData : ScriptableObject
    {
        public VendorData vendor;
        [TextArea] public string saleText;
        [TextArea] public string acceptResultText;
        [TextArea] public string rejectResultText;
        
        [Space]
        public StatEffect[] rejectedEffects;

        public abstract StatEffect[] GetActualEffects();
        public abstract StatEffect[] GetClaimedEffects();
    }
}