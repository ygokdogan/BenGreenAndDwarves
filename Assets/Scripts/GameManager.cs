using System;
using System.Linq;
using Effects;
using ScriptableObjects;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UpkeepEffects[] upkeepEffects;
    
    public AudioSource uiButtonSource;

    public int dailyAccepts;
    public int dailyRejects;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (upkeepEffects != null && upkeepEffects.Length > 0)
        {
            upkeepEffects = upkeepEffects.OrderByDescending(u => u.minAvg).ToArray();
        }
    }

    private void Start()
    {
        TimeManager.Instance.OnDayEnded += EndDay;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnDayEnded -= EndDay;
    }


    public void EndDay(int day)
    {
        int totalEncounters = dailyAccepts + dailyRejects;
        float avgAccepts = totalEncounters > 0 ?  (float)dailyAccepts / (float)totalEncounters : 0.5f;
        
        UpkeepEffects dailyUpkeep = DecideUpkeepStats(avgAccepts);
        
        DayEndUIManager.Instance.ShowDayEndSummary(day, dailyUpkeep);
        
        dailyAccepts = 0; dailyRejects = 0;
    }

    public void EndGame(StatType stat, bool isZero = true)
    {
        Debug.Log($"Game Ended: {stat} (isZero: {isZero})");
        if (GameOverUIManager.Instance != null)
        {
            GameOverUIManager.Instance.ShowGameOver(stat, isZero);
        }
    }

    private UpkeepEffects DecideUpkeepStats(float avg)
    {
        foreach (var upkeep in upkeepEffects)
        {
            if (avg >= upkeep.minAvg) return upkeep;
        }
        
        Debug.LogWarning($"No UpkeepEffects found for average: {avg}. Returning default.");
        return default;
    }
}