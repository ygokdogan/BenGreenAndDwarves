using Stats;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Lie Encounter Data", menuName = "Data/Lie Encounter")]
    public class LieEncounterData : EncounterData
    {
        public StatEffect[] claimedEffects;
        public StatEffect[] actualEffects;
        
        public override StatEffect[] GetActualEffects() => actualEffects;
    }
}