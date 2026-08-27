using System;
using Effects;
using ScriptableObjects;
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
    }

    public void ResolveEncounter(EncounterData encounter, bool accepted)
    {
        if (accepted)
        {
            foreach (StatEffect effect in encounter.GetActualEffects())
            {
                if (effect.Instant)
                {
                    StatManager.Instance.ApplyEffect(effect);
                }
                else
                {
                    PendingEffects.Instance.Schedule(encounter, effect, effect.revealDelay, encounter.delayedText);
                }
            }
        }
        else
        {
            StatManager.Instance.ApplyEffects(encounter.rejectedEffects);
        }
    }

    public void EndGame(StatType stat, bool isZero = true)
    {
        Debug.Log($"Game Ended: {stat} (isZero: {isZero})");
        if (UI.GameOverUIManager.Instance != null)
        {
            UI.GameOverUIManager.Instance.ShowGameOver(stat, isZero);
        }
    }
}