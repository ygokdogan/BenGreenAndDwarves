using System;
using Effects;
using ScriptableObjects;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AudioSource uiButtonSource;

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
            encounter.accepted = true;
            foreach (StatEffect effect in encounter.GetActualEffects())
            {
                if (effect.Instant)
                {
                    StatManager.Instance.ApplyEffect(effect, checkGameOver: false);
                }
                else
                {
                    PendingEffects.Instance.Schedule(encounter, effect, effect.revealDelay);
                }
            }
        }
        else
        {
            encounter.accepted = false;
            StatManager.Instance.ApplyEffects(encounter.rejectedEffects, checkGameOver: false);
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