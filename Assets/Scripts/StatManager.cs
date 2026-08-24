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
            if (stat.Value <= 0)
            {
                GameManager.Instance.EndGame(stat.Key);
            }
        }
    }
}