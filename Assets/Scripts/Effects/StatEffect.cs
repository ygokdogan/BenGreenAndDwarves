namespace Effects
{
    [System.Serializable]
    public struct StatEffect
    {
        public StatType type;
        public int amount;
        public int revealDelay;
        public bool Instant => revealDelay <= 0;
    }

    public enum StatType
    {
        Happiness,
        Cash,
        Storage,
        Health,
    }
}