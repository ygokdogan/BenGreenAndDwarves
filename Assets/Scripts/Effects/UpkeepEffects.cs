using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(fileName = "New Upkeep Effect", menuName = "Effects/Upkeep Effects")]
    public class UpkeepEffects : ScriptableObject
    {
        public string title;
        public string summaryText;
        public float minAvg;
        
        public StatEffect[] effects;
    }
}