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
        
        InitializeStats(50,50,50,50);
    }

    private void InitializeStats(int happiness, int health, int storage, int cash)
    {
        stats.Add(StatType.Happiness, happiness);
        stats.Add(StatType.Health, health);
        stats.Add(StatType.Storage, storage);
        stats.Add(StatType.Cash, cash);
    }

    public void ApplyEffects(StatEffect[] effects)
    {
        foreach (StatEffect effect in effects)
        {
            stats[effect.type] = Mathf.Clamp(stats[effect.type] + effect.amount, 0, 100);
        }
        CheckGameOver();
    }

    public void ApplyEffect(StatEffect effect)
    {
        stats[effect.type] = Mathf.Clamp(stats[effect.type] + effect.amount, 0, 100);
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