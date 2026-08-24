namespace Stats
{
    [System.Serializable]
    public struct StatEffect
    {
        public StatType type;
        public int amount;
    }

    public enum StatType
    {
        Happiness,
        Cash,
        Storage,
        Health,
    }
}