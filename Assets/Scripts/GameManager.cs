using System;
using ScriptableObjects;
using Stats;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public void ResolveEncounter(EncounterData encounter, bool accepted)
    {
        if (accepted)
        {
            if (encounter.revealDelay <= 0)
            {
                StatManager.Instance.ApplyEffect(encounter.GetActualEffects());
                return;
            }
            
            //PendingEffects.Instance.Schedule(encounter.GetActualEffects(), encounter.revealDelay);
        }
        else
        {
            StatManager.Instance.ApplyEffect(encounter.GetActualEffects());
        }
    }

    public void EndGame(StatType stat)
    {
        Debug.Log($"Game Ended: {stat}");
    }
}