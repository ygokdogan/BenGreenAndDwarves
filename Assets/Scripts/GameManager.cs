using System;
using System.Linq;
using Challenges;
using Effects;
using ScriptableObjects;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UpkeepEffects[] upkeepEffects;
    
    public int dailyAccepts;
    public int dailyRejects;

    private int daysToSurvive = 7;
    
    [SerializeField] private AudioClip gameWonClip;
    [SerializeField] private AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (GetComponent<GameFlowManager>() == null)
            gameObject.AddComponent<GameFlowManager>();

        if (upkeepEffects != null && upkeepEffects.Length > 0)
        {
            upkeepEffects = upkeepEffects.OrderByDescending(u => u.minAvg).ToArray();
        }
    }

    public void EndDay(int day)
    {
        if (day >= daysToSurvive)
        {
            WinGame();
            return;
        }
        
        int totalEncounters = dailyAccepts + dailyRejects;
        float avgAccepts = totalEncounters > 0 ?  (float)dailyAccepts / (float)totalEncounters : 0.5f;
        
        UpkeepEffects dailyUpkeep = DecideUpkeepStats(avgAccepts); 
        
        bool wasDayResolved = DayEndUIManager.Instance.ShowDayEndSummary(day, dailyUpkeep);
        if (!wasDayResolved) return;
        
        dailyAccepts = 0; dailyRejects = 0;
    }

    private void WinGame()
    {
        ChallengeManager.Instance?.OnDayResolved(TimeManager.Instance.CurrentDay);
        AudioManager.Instance?.PlayMusic(gameWonClip);
        GameFlowManager.Instance?.ShowGameWon();
    }

    public void EndGame(StatType stat, bool isZero = true)
    {
        Debug.Log($"Game Ended: {stat} (isZero: {isZero})");
        
        GameFlowManager.Instance?.ShowGameOver(stat, isZero);
        AudioManager.Instance?.PlayMusicInstantly(gameOverClip);
        ChallengeManager.Instance?.OnGameEnded();
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
