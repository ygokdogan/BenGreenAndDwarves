using Effects;
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class EncounterData : ScriptableObject
    {
        public VendorData vendor;
        public string vendorName;
        [TextArea] public string offerText;
        [TextArea] public string acceptResultText;
        [TextArea] public string rejectResultText;
        [Tooltip("Only for delayed effects")]
        [TextArea] public string acceptedDelayedText;
        [TextArea] public string rejectedDelayedText;
        [HideInInspector] public bool accepted;
        
        [Space]
        public StatEffect[] rejectedEffects;

        public abstract StatEffect[] GetActualEffects();
        public abstract StatEffect[] GetClaimedEffects();
    }
}