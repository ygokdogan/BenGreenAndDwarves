using System;
using System.Collections.Generic;
using Stats;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;
    public Dictionary<StatType, int> stats = new Dictionary<StatType, int>();

    public event Action<StatType, int> OnStatChanged;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;
        
        InitializeStats(50,50,50,50);
    }

    private void InitializeStats(int happiness, int health, int storage, int cash)
    {
        stats.Add(StatType.Happiness, happiness);
        stats.Add(StatType.Health, health);
        stats.Add(StatType.Storage, storage);
        stats.Add(StatType.Cash, cash);
    }

    public void ApplyEffects(StatEffect[] effects, bool checkGameOver = true)
    {
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
        stats[effect.type] = Mathf.Clamp(stats[effect.type] + effect.amount, 0, 100);
        OnStatChanged?.Invoke(effect.type, stats[effect.type]);
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

        foreach (var stat in stats)
        {
            OnStatChanged?.Invoke(stat.Key, stat.Value);
        }
    }

    public bool CheckGameOver()
    {
        foreach (var stat in stats)
        {
            if (stat.Value <= 0 || stat.Value >= 100)
            {
                GameManager.Instance.EndGame(stat.Key, stat.Value <= 0);
                return true;
            }
        }

        return false;
    }
}