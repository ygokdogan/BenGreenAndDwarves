using System;
using System.Collections.Generic;
using System.Linq;
using Challenges;
using Effects;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;
    
    public Dictionary<StatType, int> stats = new Dictionary<StatType, int>();
    public Dictionary<StatType, int> maxStats = new Dictionary<StatType, int>();

    public event Action<StatType, int, int> OnStatChanged;
    public event Action<StatType, int> OnMaxStatChanged;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;
        
        InitializeMaxStats(100, 100, 100, 100);
        InitializeStats(50,50,50,50);
    }
    
    private void InitializeMaxStats(int happiness, int health, int storage, int cash)
    {
        maxStats[StatType.Happiness] = happiness;
        maxStats[StatType.Health] = health;
        maxStats[StatType.Storage] = storage;
        maxStats[StatType.Cash] = cash;
    }

    private void InitializeStats(int happiness, int health, int storage, int cash)
    {
        stats[StatType.Happiness] = happiness;
        stats[StatType.Health] = health;
        stats[StatType.Storage] = storage;
        stats[StatType.Cash] = cash;
    }

    public void SetMax(StatType statType, int value)
    {
        maxStats[statType] = value;
        OnMaxStatChanged?.Invoke(statType, value);
    }

    public void SetCurrent(StatType statType, int value)
    {
        var oldValue = stats[statType];
        stats[statType] = Mathf.Clamp(value, 0, maxStats[statType]);
        OnStatChanged?.Invoke(statType, stats[statType], oldValue);
    }

    public void SetMaxKeepingPercentage(StatType statType, int value)
    {
        var oldCurr = stats[statType];
        float per = (float)stats[statType] / maxStats[statType];
        maxStats[statType] = value;
        stats[statType] = Mathf.RoundToInt(per * value);
        
        OnMaxStatChanged?.Invoke(statType, value);
        OnStatChanged?.Invoke(statType, stats[statType], oldCurr);
    }

    public void NormalizeToHalfOfMaximum(StatType statType)
    {
        int oldValue = stats[statType];
        stats[statType] = maxStats[statType] / 2;

        OnStatChanged?.Invoke(statType, stats[statType], oldValue);
    }

    public void NormalizeAllStats()
    {
        foreach (var stat in stats.Keys.ToList())
        {
            NormalizeToHalfOfMaximum(stat);
        }
    }

    public void ApplyEffects(StatEffect[] effects, bool checkGameOver = true)
    {
        if (effects == null || effects.Length <= 0) return;
        
        foreach (StatEffect effect in effects)
        {
            ApplyEffect(effect, checkGameOver);
        }
        if (checkGameOver)
        {
            CheckGameOver();
        }
    }

    public void ApplyEffect(StatEffect effect, bool checkGameOver = true)
    {
        var oldValue = stats[effect.type];
        stats[effect.type] = Mathf.Clamp(stats[effect.type] + effect.amount, 0, maxStats[effect.type]);
        OnStatChanged?.Invoke(effect.type, stats[effect.type], oldValue);
        
        if (checkGameOver)
        {
            CheckGameOver();
        }
    }

    public void ResetStats(int happiness = 50, int health = 50, int storage = 50, int cash = 50)
    {
        stats[StatType.Happiness] = happiness;
        stats[StatType.Health] = health;
        stats[StatType.Storage] = storage;
        stats[StatType.Cash] = cash;
        
        OnStatChanged?.Invoke(StatType.Happiness, stats[StatType.Happiness], happiness);
        OnStatChanged?.Invoke(StatType.Health, stats[StatType.Health], health);
        OnStatChanged?.Invoke(StatType.Storage, stats[StatType.Storage], storage);
        OnStatChanged?.Invoke(StatType.Cash, stats[StatType.Cash], cash);
    }

    public bool CheckGameOver()
    {
        foreach (var stat in stats)
        {
            if (stat.Value <= 0 || stat.Value >= maxStats[stat.Key])
            {
                if (ChallengeManager.Instance &&
                    ChallengeManager.Instance.TryPreventGameOver(stat.Key))
                {
                    continue;
                }

                GameManager.Instance.EndGame(stat.Key, stat.Value <= 0);
                return true;
            }
        }

        return false;
    }
}
