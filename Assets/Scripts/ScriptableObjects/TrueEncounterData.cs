using Effects;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New True Encounter Data", menuName = "Data/True Encounter")]
    public class TrueEncounterData : EncounterData
    {
        public StatEffect[] effects;

        public override StatEffect[] GetActualEffects() => effects;
        public override StatEffect[] GetClaimedEffects() => effects;
    }
}