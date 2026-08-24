using System;
using System.Collections.Generic;
using Stats;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;
    public Dictionary<StatType, int> stats = new Dictionary<StatType, int>();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        stats.Add(StatType.Happiness, 50);
        stats.Add(StatType.Health, 50);
        stats.Add(StatType.Storage, 50);
        stats.Add(StatType.Cash, 50);
    }

    private void Update()
    {
        Debug.Log($"Happiness: {stats[StatType.Happiness]}");
        Debug.Log($"Health: {stats[StatType.Health]}");
        Debug.Log($"Storage: {stats[StatType.Storage]}");
        Debug.Log($"Cash: {stats[StatType.Cash]}");
    }

    public void ApplyEffect(StatEffect[] effects)
    {
        foreach (StatEffect effect in effects)
        {
            stats[effect.type] = Mathf.Clamp(stats[effect.type] + effect.amount, 0, 100);
        }
        CheckGameOver();
    }

    public void CheckGameOver()
    {
        foreach (var stat in stats)
        {
            if (stat.Value <= 0 || stat.Value >= 100)
            {
                GameManager.Instance.EndGame(stat.Key);
            }
        }
    }
}